#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
namespace MessagePack.Unity.Editor
{
    [CustomEditor(typeof(MessagePackWindow))]
    internal class MessagePackWindowEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var processInitialized = serializedObject.FindProperty("processInitialized");
            var mpcArgument = serializedObject.FindProperty("mpcArgument");
            var isDotnetInstalled = serializedObject.FindProperty("isDotnetInstalled");
            var isInstalledMpc = serializedObject.FindProperty("isInstalledMpc");
            var invokingMpc = serializedObject.FindProperty("invokingMpc");

            var root = new VisualElement();
            if (!processInitialized.boolValue) {
                root.Add(new Label("检测.NET Core SDK/CodeGen 安装状态"));
                return root;
            }
            if (mpcArgument.boxedValue == null) {
                return root;
            }
            if (!isDotnetInstalled.boolValue) {
                root.Add(new Label(".NET Core SDK 未找到"));
                root.Add(new Label("MessagePack CodeGen 需要 .NET Core Runtime."));

                var oiBtn = new Button();
                oiBtn.text = "打开 .NET Core install page.";
                oiBtn.RegisterCallback<ClickEvent>(e =>
                {
                    Application.OpenURL("https://dotnet.microsoft.com/download");
                });
                root.Add(oiBtn);
                return root;
            }
            if (!isInstalledMpc.boolValue) {
                root.Add(new Label("MessagePack CodeGen未安装"));
                var imcBtn = new Button();
                imcBtn.text = "安装MessagePack CodeGen";
                async void EventCallback(ClickEvent e)
                {
                    isInstalledMpc.boolValue = true;
                    try {
                        var log = await ProcessHelper.InstallMpc();
                        if (!string.IsNullOrWhiteSpace(log)) {
                            UnityEngine.Debug.Log(log);
                        }
                        if (log != null && log.Contains("error")) {
                            isInstalledMpc.boolValue = false;
                        } else {
                            isInstalledMpc.boolValue = true;
                        }
                    }
                    finally {
                        isInstalledMpc.boolValue = false;
                    }
                }
                imcBtn.RegisterCallback<ClickEvent>(EventCallback);
                root.Add(imcBtn);
                return root;
            }
            
            root.Add(new Label("-i input path(csproj or directory):"));
            root.Add(new PropertyField(mpcArgument.FindPropertyRelative("Input")));
            root.Add(new Label("-o output filepath(.cs) or directory(multiple):"));
            root.Add(new PropertyField(mpcArgument.FindPropertyRelative("Output")));
            root.Add(new Label("-m(optional) use map mode:"));
            var tog = new PropertyField(mpcArgument.FindPropertyRelative("UseMapMode"));
            root.Add(tog);
            tog.RegisterValueChangeCallback(tempUseMapNode =>
            {
                ((MpcArgument)mpcArgument.boxedValue).Save();
            });

            root.Add(new Label("-c(optional) conditional compiler symbols(split with ','):"));
            root.Add(new PropertyField(mpcArgument.FindPropertyRelative("ConditionalSymbol")));
            
            root.Add(new Label("-r(optional) generated resolver name:"));
            root.Add(new PropertyField(mpcArgument.FindPropertyRelative("ResolverName")));
            
            root.Add(new Label("-n(optional) namespace root name:"));
            root.Add(new PropertyField(mpcArgument.FindPropertyRelative("Namespace")));
            
            root.Add(new Label("-ms(optional) Generate #if-- files by symbols, split with ','"));
            root.Add(new PropertyField(mpcArgument.FindPropertyRelative("MultipleIfDirectiveOutputSymbols")));

            var generateBtn = new Button();
            generateBtn.text = "生成";
            async void Callback(ClickEvent e)
            {
                var commnadLineArguments = mpcArgument.ToString();
                UnityEngine.Debug.Log("Generate MessagePack Files, command:" + commnadLineArguments);

                invokingMpc.boolValue = true;
                try {
                    var log = await ProcessHelper.InvokeProcessStartAsync("mpc", commnadLineArguments);
                    UnityEngine.Debug.Log(log);
                }
                finally {
                    invokingMpc.boolValue = false;
                }
            }
            generateBtn.RegisterCallback<ClickEvent>(Callback);
            root.Add(generateBtn);
            return root;
        }
    }
}
#endif