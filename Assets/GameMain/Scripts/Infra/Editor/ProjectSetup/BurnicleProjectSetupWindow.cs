using UnityEditor;
using UnityEngine;

namespace Template
{
    /// <summary>
    /// 新项目设置窗口：改产品名 / 公司名 / 包名 / 命名空间根 / 启动场景名。
    /// </summary>
    public class BurnicleProjectSetupWindow : EditorWindow
    {
        private const string MenuPath = "Tools/Burnicle/新建项目设置";

        private string _productName;
        private string _companyName;
        private string _bundleIdentifier;
        private string _namespaceRoot;
        private string _launchSceneName;

        [MenuItem(MenuPath)]
        public static void Open()
        {
            var window = GetWindow<BurnicleProjectSetupWindow>("新建项目设置");
            window.minSize = new Vector2(560f, 460f);
            window.Show();
        }

        private void OnEnable()
        {
            LoadFromRecord();
        }

        private void LoadFromRecord()
        {
            var record = BurnicleProjectSetup.Instance;
            _productName = record.ProductName;
            _companyName = record.CompanyName;
            _bundleIdentifier = record.BundleIdentifier;
            _namespaceRoot = record.NamespaceRoot;
            _launchSceneName = record.LaunchSceneName;
        }

        private void OnGUI()
        {
            var record = BurnicleProjectSetup.Instance;

            EditorGUILayout.LabelField("项目信息", EditorStyles.boldLabel);
            _productName = EditorGUILayout.TextField("产品名", _productName);
            _companyName = EditorGUILayout.TextField("公司名", _companyName);
            _bundleIdentifier = EditorGUILayout.TextField("包名", _bundleIdentifier);
            EditorGUILayout.HelpBox("包名写进 Unity PlayerSettings（Android / iOS / Standalone / WebGL）。" +
                                    "各平台构建配置里的包名如有独立设置，需要自己同步。", MessageType.None);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("代码与场景", EditorStyles.boldLabel);
            _namespaceRoot = EditorGUILayout.TextField("命名空间根", _namespaceRoot);
            _launchSceneName = EditorGUILayout.TextField("启动场景名", _launchSceneName);
            EditorGUILayout.HelpBox(
                $"命名空间根会重写 {BurnicleProjectSetupService.ScriptsRoot} 下的源码命名空间，" +
                "场景里各种 Helper / 流程的类型全名也会一起改。\n" +
                "场景文件名跟着「启动场景名」改，Build Settings 同步更新。", MessageType.None);

            if (record.Completed)
            {
                EditorGUILayout.HelpBox(
                    $"已经执行过一次（{record.AppliedAt}）。当前记录的命名空间根是 '{record.NamespaceRoot}'、" +
                    $"启动场景 '{record.LaunchSceneName}'。再次执行会从这些旧值改到上面填的新值。", MessageType.Warning);
            }

            EditorGUILayout.Space();
            var valid = BurnicleProjectSetupService.IsValidNamespaceRoot(_namespaceRoot)
                        && !string.IsNullOrWhiteSpace(_launchSceneName);
            using (new EditorGUI.DisabledScope(!valid))
            {
                if (GUILayout.Button("应用设置", GUILayout.Height(32f)))
                {
                    Apply();
                }
            }

            if (!valid)
            {
                EditorGUILayout.HelpBox("命名空间根必须是合法的 C# 命名空间，启动场景名不能为空。", MessageType.Error);
            }
        }

        private void Apply()
        {
            var report = BurnicleProjectSetupService.Apply(_productName, _companyName, _bundleIdentifier,
                _namespaceRoot, _launchSceneName);

            LoadFromRecord();
            EditorUtility.DisplayDialog("新建项目设置", string.Join("\n", report), "好");
        }
    }
}
