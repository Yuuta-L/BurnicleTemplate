using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 项目初始化向导窗口。自动弹出见 <see cref="BurnicleSetupWizardBootstrap"/>，手动入口见菜单。
/// 删除整个 Assets/BurnicleSetupWizard 文件夹即可移除向导。
/// </summary>
public class BurnicleSetupWizardWindow : EditorWindow
{
    private const string WindowTitle = "项目初始化向导";
    private const string MenuPath = "Tools/项目初始化向导";

    private static GUIStyle s_TitleStyle;
    private static GUIStyle s_SectionStyle;
    private static GUIStyle s_BlockStyle;

    private BurnicleSetupData _data;
    private string _oldNamespaceRoot;
    private string _oldSceneName;
    private int _namespaceFileCount;
    private Vector2 _scroll;

    // 脱离模板仓库：作业放静态字段，关掉窗口再打开还能看到进度
    private static BurnicleGitJob s_Job;
    private string _newRemoteUrl = "";
    private string _originUrl;
    private bool _pushAfterDetach = true;
    private string _jobLog = "";
    private Vector2 _jobScroll;

    private static bool JobRunning => s_Job != null && !s_Job.IsDone;

    [MenuItem(MenuPath, false, 560)]
    public static void Open()
    {
        var window = GetWindow<BurnicleSetupWizardWindow>(WindowTitle);
        window.minSize = new Vector2(620f, 560f);
        window.Show();
    }

    private void OnEnable()
    {
        Reload();
        if (JobRunning)
        {
            EditorApplication.update += PumpJob;
        }
    }

    private void OnDisable()
    {
        EditorApplication.update -= PumpJob;
    }

    private void PumpJob()
    {
        if (s_Job == null)
        {
            EditorApplication.update -= PumpJob;
            return;
        }

        var changed = false;
        while (s_Job.TryDequeue(out var line))
        {
            AppendJobLog(line);
            changed = true;
        }

        if (s_Job.IsDone)
        {
            EditorApplication.update -= PumpJob;
            if (s_Job.Succeeded)
            {
                _originUrl = _newRemoteUrl;
                _newRemoteUrl = "";
            }

            changed = true;
        }

        if (changed)
        {
            Repaint();
        }
    }

    private void AppendJobLog(string line)
    {
        // 日志只保留最近一段，避免长时间推送把字符串无限拉长
        _jobLog = _jobLog.Length + line.Length > 12000 ? line : _jobLog + line;
        _jobLog += "\n";
    }

    private void Reload()
    {
        var current = BurnicleSetupState.Load();
        _oldNamespaceRoot = current.NamespaceRoot;
        _oldSceneName = current.LaunchSceneName;
        _data = current;
        _namespaceFileCount = BurnicleProjectSetupService.CountFilesWithNamespaceRoot(_oldNamespaceRoot);
        _originUrl = BurnicleGitRemoteService.GetOriginUrl();
    }

    private void OnGUI()
    {
        // GUIStyle 在 OnGUI 里首次创建，避免在 GUI 上下文之外访问 EditorStyles
        if (s_TitleStyle == null)
        {
            s_TitleStyle = new GUIStyle(EditorStyles.boldLabel) { fontSize = 15 };
            s_SectionStyle = new GUIStyle(EditorStyles.boldLabel) { fontSize = 12 };
            s_BlockStyle = new GUIStyle(GUI.skin.box) { padding = new RectOffset(10, 10, 8, 10) };
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField(WindowTitle, s_TitleStyle);
        EditorGUILayout.HelpBox(
            "这个向导把模板改成你自己的项目：改 PlayerSettings、重命名启动场景、替换代码命名空间根。\n" +
            "不需要向导时，删除 Assets/BurnicleSetupWizard 整个文件夹即可。",
            MessageType.Info);

        _scroll = EditorGUILayout.BeginScrollView(_scroll, GUILayout.ExpandHeight(true));

        EditorGUILayout.BeginVertical(s_BlockStyle);
        EditorGUILayout.LabelField("设置块 1：项目初始化", s_SectionStyle);
        DrawProjectSection();
        DrawCodeSection();
        DrawPreviewSection();
        DrawSetupActions();
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical(s_BlockStyle);
        EditorGUILayout.LabelField("设置块 2：远程仓库", s_SectionStyle);
        DrawRemoteSection();
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndScrollView();
    }

    private void DrawProjectSection()
    {
        EditorGUILayout.LabelField("项目信息", EditorStyles.boldLabel);
        _data.ProductName = EditorGUILayout.TextField("产品名", _data.ProductName);
        _data.CompanyName = EditorGUILayout.TextField("公司名", _data.CompanyName);
        _data.BundleIdentifier =
            BurnicleProjectSetupService.BuildBundleIdentifier(_data.CompanyName, _data.ProductName);
        using (new EditorGUI.DisabledScope(true))
        {
            EditorGUILayout.TextField("包名", _data.BundleIdentifier);
        }

        EditorGUILayout.HelpBox(
            "包名不能手填，由「公司名」+「产品名」自动组合成 com.{公司名}.{产品名}（只读），" +
            "并写进 Unity PlayerSettings（Android / iOS / Standalone / WebGL），" +
            "各平台构建配置里的包名如有独立设置需要自己同步。",
            MessageType.None);
    }

    private void DrawCodeSection()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("代码与场景", EditorStyles.boldLabel);
        _data.NamespaceRoot = EditorGUILayout.TextField("命名空间根", _data.NamespaceRoot);
        _data.LaunchSceneName = EditorGUILayout.TextField("启动场景名", _data.LaunchSceneName);
        EditorGUILayout.HelpBox(
            $"命名空间根会重写 {BurnicleProjectSetupService.ScriptsRoot} 下的源码命名空间，" +
            "场景里各种 Helper / 流程的类型全名也会一起改；场景文件名跟「启动场景名」改，Build Settings 同步更新。",
            MessageType.None);

        if (!BurnicleProjectSetupService.IsValidNamespaceRoot(_data.NamespaceRoot))
        {
            EditorGUILayout.HelpBox("命名空间根必须是合法的 C# 命名空间。", MessageType.Error);
        }

        if (string.IsNullOrWhiteSpace(_data.LaunchSceneName))
        {
            EditorGUILayout.HelpBox("启动场景名不能为空。", MessageType.Error);
        }
    }

    private void DrawPreviewSection()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("应用预览", EditorStyles.boldLabel);

        var changes = BuildPreview();
        if (changes.Count == 0)
        {
            EditorGUILayout.HelpBox("没有检测到需要应用的改动。", MessageType.None);
            return;
        }

        foreach (var change in changes)
        {
            EditorGUILayout.LabelField($"· {change}", EditorStyles.wordWrappedLabel);
        }
    }

    private List<string> BuildPreview()
    {
        var changes = new List<string>();

        if (!string.IsNullOrEmpty(_data.ProductName) && _data.ProductName != PlayerSettings.productName)
        {
            changes.Add($"产品名：{PlayerSettings.productName} → {_data.ProductName}");
        }

        if (!string.IsNullOrEmpty(_data.CompanyName) && _data.CompanyName != PlayerSettings.companyName)
        {
            changes.Add($"公司名：{PlayerSettings.companyName} → {_data.CompanyName}");
        }

        if (!string.IsNullOrEmpty(_data.BundleIdentifier))
        {
            var currentIdentifier = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);
            if (currentIdentifier != _data.BundleIdentifier)
            {
                changes.Add($"包名：{currentIdentifier} → {_data.BundleIdentifier}");
            }
        }

        var oldScenePath = $"{BurnicleProjectSetupService.ScenesFolder}/{_oldSceneName}.unity";
        var newScenePath = $"{BurnicleProjectSetupService.ScenesFolder}/{_data.LaunchSceneName}.unity";
        if (oldScenePath != newScenePath)
        {
            changes.Add(File.Exists(oldScenePath)
                ? $"启动场景：{oldScenePath} → {newScenePath}"
                : $"启动场景：{_data.LaunchSceneName}（旧场景 {oldScenePath} 不存在，只更新记录）");
        }

        if (_oldNamespaceRoot != _data.NamespaceRoot)
        {
            changes.Add($"命名空间根：{_oldNamespaceRoot} → {_data.NamespaceRoot}" +
                        $"（改写 {_namespaceFileCount} 个源文件）");
        }

        if (BurnicleSetupState.Completed)
        {
            changes.Add("完成标记：已存在（应用后会更新记录）");
        }

        return changes;
    }

    private void DrawRemoteSection()
    {
        var git = BurnicleGitRemoteService.FindGit();
        EditorGUILayout.LabelField("当前 origin", string.IsNullOrEmpty(_originUrl) ? "（无）" : _originUrl);
        if (git == null)
        {
            EditorGUILayout.HelpBox("没找到 git.exe：请安装 Git 或把它加入 PATH 后重启 Unity。", MessageType.Error);
        }

        using (new EditorGUI.DisabledScope(JobRunning))
        {
            _newRemoteUrl = EditorGUILayout.TextField("新仓库地址", _newRemoteUrl);
            _pushAfterDetach = EditorGUILayout.Toggle("完成后立即推送", _pushAfterDetach);
        }

        EditorGUILayout.HelpBox(
            "旧 origin 直接丢弃，把当前分支重置成单个初始提交（模板的提交历史不再保留），" +
            "然后推送到新仓库。目标仓库需事先建好且为空。只改 .git 内部状态，不动工作区文件。\n" +
            "框架子模块保持原样：新仓库 clone 时用 --recurse-submodules，共享者仍需能访问框架库。",
            MessageType.None);

        if (!string.IsNullOrWhiteSpace(_newRemoteUrl) && !string.IsNullOrWhiteSpace(git) &&
            !BurnicleGitRemoteService.IsValidRemoteUrl(_newRemoteUrl.Trim()))
        {
            EditorGUILayout.HelpBox("只支持 https:// 开头的仓库地址（SSH 地址不行）。", MessageType.Error);
        }

        var canDetach = !JobRunning && !string.IsNullOrWhiteSpace(git) &&
                        BurnicleGitRemoteService.IsValidRemoteUrl(_newRemoteUrl.Trim());
        using (new EditorGUI.DisabledScope(!canDetach))
        {
            if (GUILayout.Button("脱离模板仓库", GUILayout.Height(28f)))
            {
                ConfirmDetach(_newRemoteUrl.Trim());
            }
        }

        DrawJobLog();
    }

    private void DrawJobLog()
    {
        if (s_Job == null)
        {
            return;
        }

        EditorGUILayout.Space();
        if (JobRunning)
        {
            EditorGUILayout.LabelField("状态：执行中…（推送期间请勿关闭窗口）", EditorStyles.boldLabel);
            if (GUILayout.Button("取消", GUILayout.Height(22f)))
            {
                s_Job.Cancel();
            }
        }
        else
        {
            EditorGUILayout.LabelField(s_Job.Succeeded ? "状态：完成" : $"状态：失败 — {s_Job.FailureReason}",
                EditorStyles.boldLabel);
        }

        _jobScroll = EditorGUILayout.BeginScrollView(_jobScroll, GUILayout.Height(140f));
        using (new EditorGUI.DisabledScope(true))
        {
            EditorGUILayout.TextArea(_jobLog);
        }

        EditorGUILayout.EndScrollView();
    }

    private void ConfirmDetach(string remoteUrl)
    {
        var push = _pushAfterDetach
            ? "\n\n完成后会执行：git push -u origin <当前分支>（需要登录时会弹出 Git 凭据窗口）"
            : "\n\n未勾选推送，完成后需要自己执行 git push。";

        var confirmed = EditorUtility.DisplayDialog(WindowTitle,
            "确认脱离模板仓库？（不可撤销）\n\n" +
            $"· 当前分支会重置成单个初始提交（提交信息：“初始提交：{_data.ProductName}”）\n" +
            $"· 旧 origin（{_originUrl ?? "无"}）会被丢弃，改为 {remoteUrl}\n" +
            "· 工作区文件不会改动，不会触发重新导入\n" +
            "· 目标仓库必须已创建且为空，否则推送会被拒绝" + push,
            "确认脱离", "取消");

        if (!confirmed)
        {
            return;
        }

        _jobLog = "";
        s_Job = BurnicleGitRemoteService.Detach(remoteUrl, _pushAfterDetach,
            $"初始提交：{_data.ProductName}");
        EditorApplication.update += PumpJob;
    }

    private void DrawSetupActions()
    {
        EditorGUILayout.Space();
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("稍后再说", GUILayout.Height(28f)))
            {
                Close();
            }

            var valid = BurnicleProjectSetupService.IsValidNamespaceRoot(_data.NamespaceRoot)
                        && !string.IsNullOrWhiteSpace(_data.LaunchSceneName)
                        && !JobRunning;
            using (new EditorGUI.DisabledScope(!valid))
            {
                if (GUILayout.Button("应用设置", GUILayout.Height(28f)))
                {
                    Apply();
                }
            }
        }
    }

    private void Apply()
    {
        var report = BurnicleProjectSetupService.Apply(_data.ProductName, _data.CompanyName,
            _data.NamespaceRoot, _data.LaunchSceneName);

        EditorUtility.DisplayDialog(WindowTitle, string.Join("\n", report), "好");

        if (BurnicleSetupState.Completed)
        {
            Close();
        }
        else
        {
            Reload();
        }
    }
}
