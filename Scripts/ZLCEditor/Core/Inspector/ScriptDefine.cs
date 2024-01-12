using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
namespace ZLCEditor.Inspector
{
    /// <summary>
    /// 条件编译项
    /// </summary>
    [Serializable]
    public class ScriptDefine
    {
        public string name;
        public bool enabled;
    }

    [CustomPropertyDrawer(typeof(ScriptDefine))]
    public class ScriptDefineEditor : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            var name = property.FindPropertyRelative("name");
            var enabled = property.FindPropertyRelative("enabled");
            var nameVe = new PropertyField(name);
            var enabledVe = new PropertyField(enabled);
            enabledVe.RegisterValueChangeCallback(e =>
            {
                var value = e.changedProperty.boolValue;
                var contractDefine = name.stringValue;
                var defines = PlayerSettings
                    .GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
                var hasPartialDefine = defines
                    .Contains(contractDefine);
                if (value && !hasPartialDefine) {
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup,
                        $"{defines};{contractDefine}");
                } else if (!value && hasPartialDefine) {
                    defines = defines.Remove(defines.IndexOf(contractDefine, StringComparison.Ordinal),
                        contractDefine.Length);
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup,
                        defines);
                }
            });
            root.Add(nameVe);
            root.Add(enabledVe);
            enabled.boolValue = PlayerSettings
                .GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup)
                .Contains(name.stringValue);
            property.serializedObject.ApplyModifiedProperties();
            return root;
        }
    }
}