using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using ZLCEditor.Inspector.VisualElements;
using ZLCEngine.Inspector;
namespace ZLCEditor.Inspector
{
    /// <summary>
    ///     基础的编辑器代码，目前会检测需要展示在Inspector上的方法
    /// </summary>
    [CustomEditor(typeof(object), true)]
    [CanEditMultipleObjects]
    public class BaseZLCEditor : Editor
    {

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement customInspector = new VisualElement();
            customInspector.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(Constant.ZLC_EDITOR_USS));
            // -- header begin --
            /*var headerLabel = new Label("ZLC编辑器");
            headerLabel.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(Path.Combine(Constant.USSPath, "ZLCEditor.uss")));
            customInspector.Add(headerLabel);*/

            // -- header end --

            // 添加默认的ui
            SerializedProperty iterator = serializedObject.GetIterator();
            for (bool enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false) {
                if (iterator.propertyPath == "m_Script") {
                    continue;
                }
                using (new EditorGUI.DisabledScope(false)) {
                    customInspector.Add(new ZLCPropertyField(iterator));
                }
            }

            // -- 检测各个方法是否有被可序列化的特性，如果有则按对应的特性进行序列化，没有则跳过
            IEnumerable<MethodInfo> methods = target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(t => Attribute.IsDefined(t, typeof(AnySerializableAttribute), true));
            foreach (MethodInfo method in methods) {
                AnySerializableAttribute attribute = method.GetCustomAttribute<AnySerializableAttribute>(true);
                Type drawerType = ScriptAttributeUtilityWrapper.GetDrawerTypeForType(attribute.GetType());
                if (drawerType != null) {
                    IAnySerializableAttributeEditor drawer = (IAnySerializableAttributeEditor)Activator.CreateInstance(drawerType);
                    VisualElement methodGUI = drawer.CreateGUI(attribute, method, target);
                    if (methodGUI != null)
                        customInspector.Add(methodGUI);
                }
            }

            return customInspector;
        }
    }
}