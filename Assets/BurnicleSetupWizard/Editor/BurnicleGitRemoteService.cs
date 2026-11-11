using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 「脱离模板仓库」的执行逻辑：换掉 origin、把当前分支重置成单个初始提交、可选首次推送。
/// 只改 .git 内部状态（临时 index + commit-tree + update-ref），不碰工作区文件，
/// 因此不会触发资源重新导入或重新编译。
/// </summary>
internal static class BurnicleGitRemoteService
{
    private const string TempIndexRelativePath = "Library/Locus/tmp/burnicle-detach.index";

    private static string s_GitPath;
    private static bool s_GitSearched;

    public static string ProjectRoot =>
        Directory.GetParent(Application.dataPath)?.FullName ?? Directory.GetCurrentDirectory();

    public static string FindGit()
    {
        if (s_GitSearched)
        {
            return s_GitPath;
        }

        s_GitSearched = true;
        var pathVariable = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
        foreach (var directory in pathVariable.Split(Path.PathSeparator))
        {
            if (string.IsNullOrWhiteSpace(directory))
            {
                continue;
            }

            var candidate = Path.Combine(directory.Trim(), "git.exe");
            if (File.Exists(candidate))
            {
                s_GitPath = candidate;
                return s_GitPath;
            }
        }

        // Unity 从桌面启动时可能拿不到安装后才更新的 PATH
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        foreach (var directory in new[]
                 {
                     @"C:\Program Files\Git\cmd",
                     @"C:\Program Files (x86)\Git\cmd",
                     @"D:\Git\cmd",
                     @"D:\Program Files\Git\cmd",
                     Path.Combine(localAppData, @"Programs\Git\cmd"),
                 })
        {
            var candidate = Path.Combine(directory, "git.exe");
            if (File.Exists(candidate))
            {
                s_GitPath = candidate;
                break;
            }
        }

        return s_GitPath;
    }

    public static bool IsValidRemoteUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uri) && uri.Scheme == Uri.UriSchemeHttps;
    }

    /// <summary>
    /// 读当前 origin 地址；没有 origin 或 git 不可用时返回 null。
    /// </summary>
    public static string GetOriginUrl()
    {
        var git = FindGit();
        if (git == null)
        {
            return null;
        }

        var output = RunCapture(git, new[] { "remote", "get-url", "origin" });
        return string.IsNullOrEmpty(output) ? null : output;
    }

    public static BurnicleGitJob Detach(string newRemoteUrl, bool push, string commitMessage)
    {
        var job = new BurnicleGitJob();
        Task.Run(() => RunDetach(job, newRemoteUrl, push, commitMessage));
        return job;
    }

    private static void RunDetach(BurnicleGitJob job, string newRemoteUrl, bool push, string commitMessage)
    {
        var git = FindGit();
        if (git == null)
        {
            job.Append("找不到 git.exe：请安装 Git，或把它加入 PATH 后重启 Unity。");
            job.Finish(false, "找不到 git.exe");
            return;
        }

        var branch = RunCapture(git, new[] { "symbolic-ref", "--quiet", "--short", "HEAD" }) ?? "master";
        job.Append($"当前分支：{branch}");

        var tempIndexDirectory = Path.Combine(ProjectRoot, "Library/Locus/tmp");
        var tempIndex = Path.Combine(tempIndexDirectory, Path.GetFileName(TempIndexRelativePath));
        var indexEnv = new Dictionary<string, string> { ["GIT_INDEX_FILE"] = tempIndex };

        try
        {
            // 先把工作区快照做成提交，再动 origin：失败时远端配置还是干净的
            Directory.CreateDirectory(tempIndexDirectory);
            if (Run(job, git, new[] { "read-tree", "HEAD" }, indexEnv) != 0 ||
                Run(job, git, new[] { "add", "-A" }, indexEnv) != 0 ||
                !TryRun(job, git, new[] { "write-tree" }, indexEnv, out var tree) ||
                !TryRun(job, git, new[] { "commit-tree", tree, "-m", commitMessage }, null, out var commit))
            {
                job.Finish(false, "生成初始提交失败（若提示缺少身份，请先配置 git 的 user.name / user.email）");
                return;
            }

            if (Run(job, git, new[] { "update-ref", $"refs/heads/{branch}", commit }, null) != 0)
            {
                job.Finish(false, "重置分支失败");
                return;
            }

            Run(job, git, new[] { "reset", "--quiet" }, null);
            job.Append($"已把 {branch} 重置为单个初始提交 {commit}，模板提交历史不再保留");

            var currentRemote = RunCapture(git, new[] { "remote", "get-url", "origin" });
            var setRemote = currentRemote == null
                ? new[] { "remote", "add", "origin", newRemoteUrl }
                : new[] { "remote", "set-url", "origin", newRemoteUrl };
            if (Run(job, git, setRemote, null) != 0)
            {
                job.Finish(false, "设置 origin 失败");
                return;
            }

            job.Append($"origin：{currentRemote ?? "（原本没有）"} → {newRemoteUrl}");

            if (!push)
            {
                job.Finish(true, null);
                return;
            }

            job.Append("开始首次推送（如需登录会在弹窗里要求凭据）…");
            if (Run(job, git, new[] { "push", "-u", "origin", branch }, null) != 0)
            {
                job.Finish(false, "首次推送失败：确认目标仓库已创建且为空，以及凭据可用");
                return;
            }

            job.Finish(true, null);
        }
        catch (OperationCanceledException)
        {
            job.Finish(false, "已取消");
        }
        catch (Exception exception)
        {
            job.Append(exception.Message);
            job.Finish(false, exception.Message);
        }
        finally
        {
            TryDelete(tempIndex);
        }
    }

    private static void TryDelete(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch (IOException)
        {
            // 删不掉就留着，本来就在 Library 下，下次会覆盖
        }
    }

    private static string RunCapture(string git, string[] args)
    {
        try
        {
            using var process = Process.Start(CreateStartInfo(git, args, null));
            if (process == null)
            {
                return null;
            }

            var output = process.StandardOutput.ReadToEnd().Trim();
            process.StandardError.ReadToEnd();
            process.WaitForExit();
            return process.ExitCode == 0 ? output : null;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private static bool TryRun(BurnicleGitJob job, string git, string[] args,
        Dictionary<string, string> extraEnvironment, out string captured)
    {
        var exitCode = Run(job, git, args, extraEnvironment, out captured);
        return exitCode == 0 && !string.IsNullOrEmpty(captured);
    }

    private static int Run(BurnicleGitJob job, string git, string[] args,
        Dictionary<string, string> extraEnvironment)
    {
        return Run(job, git, args, extraEnvironment, out _);
    }

    private static int Run(BurnicleGitJob job, string git, string[] args,
        Dictionary<string, string> extraEnvironment, out string captured)
    {
        captured = null;
        job.Append($"$ git {string.Join(" ", args)}");

        var standardOutput = new StringBuilder();
        try
        {
            using var process = new Process { StartInfo = CreateStartInfo(git, args, extraEnvironment) };
            process.OutputDataReceived += (_, e) =>
            {
                if (e.Data == null)
                {
                    return;
                }

                lock (standardOutput)
                {
                    standardOutput.AppendLine(e.Data);
                }

                job.Append(e.Data);
            };
            process.ErrorDataReceived += (_, e) =>
            {
                if (e.Data != null)
                {
                    job.Append(e.Data);
                }
            };

            if (!process.Start())
            {
                job.Append("无法启动 git 进程");
                return -1;
            }

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            while (!process.WaitForExit(150))
            {
                if (!job.Token.IsCancellationRequested)
                {
                    continue;
                }

                try
                {
                    process.Kill();
                }
                catch (Exception)
                {
                    // 进程已经退出
                }

                throw new OperationCanceledException(job.Token);
            }

            process.WaitForExit(); // 等异步输出读完
            lock (standardOutput)
            {
                captured = standardOutput.ToString().Trim();
            }

            return process.ExitCode;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            job.Append(exception.Message);
            return -1;
        }
    }

    private static ProcessStartInfo CreateStartInfo(string git, string[] args,
        Dictionary<string, string> extraEnvironment)
    {
        var info = new ProcessStartInfo(git)
        {
            WorkingDirectory = ProjectRoot,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8,
        };

        foreach (var arg in args)
        {
            info.ArgumentList.Add(arg);
        }

        // 没终端可交互：缺凭据时快速失败，而不是在 Editor 里静默挂死
        info.EnvironmentVariables["GIT_TERMINAL_PROMPT"] = "0";
        if (extraEnvironment == null)
        {
            return info;
        }

        foreach (var pair in extraEnvironment)
        {
            info.EnvironmentVariables[pair.Key] = pair.Value;
        }

        return info;
    }
}

/// <summary>
/// 一次 git 作业的进度与输出。后台线程写，主线程（EditorApplication.update）读。
/// </summary>
internal sealed class BurnicleGitJob
{
    private readonly ConcurrentQueue<string> _lines = new ConcurrentQueue<string>();
    private readonly CancellationTokenSource _cancellation = new CancellationTokenSource();

    public bool IsDone { get; private set; }
    public bool Succeeded { get; private set; }
    public string FailureReason { get; private set; }

    internal CancellationToken Token => _cancellation.Token;

    public void Cancel()
    {
        if (!IsDone)
        {
            _cancellation.Cancel();
        }
    }

    public bool TryDequeue(out string line)
    {
        return _lines.TryDequeue(out line);
    }

    internal void Append(string line)
    {
        if (!string.IsNullOrEmpty(line))
        {
            _lines.Enqueue(line);
        }
    }

    internal void Finish(bool succeeded, string failureReason)
    {
        Succeeded = succeeded;
        FailureReason = failureReason;
        IsDone = true;
    }
}
