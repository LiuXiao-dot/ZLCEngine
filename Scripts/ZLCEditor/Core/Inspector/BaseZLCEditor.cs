using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using ZLCEngine.Inspector;
namespace ZLCEditor.Inspector
{
    /// <summary>
    /// 基础的编辑器代码，目前会检测需要展示在Inspector上的方法
    /// </summary>
    [CustomEditor(typeof(object), true)]
    [CanEditMultipleObjects]
    public class BaseZLCEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement customInspector = new VisualElement();
            customInspector.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(Path.Combine(Constant.USSPath, "Common.uss")));
            // -- header begin --
            /*var headerLabel = new Label("ZLC编辑器");
            headerLabel.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(Path.Combine(Constant.USSPath, "ZLCEditor.uss")));
            customInspector.Add(headerLabel);*/
            
            // -- header end --
            
            // 添加默认的ui
            var iterator = serializedObject.GetIterator();
            for (bool enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false)
            {
                using (new EditorGUI.DisabledScope("m_Script" == iterator.propertyPath))
                    customInspector.Add(new PropertyField(iterator));
            }

            // -- 检测各个方法是否有被可序列化的特性，如果有则按对应的特性进行序列化，没有则跳过
            var methods = target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(t => Attribute.IsDefined(t,typeof(AnySerializableAttribute),true));
            foreach (var method in methods) {
                var attribute = method.GetCustomAttribute<AnySerializableAttribute>(true);
                var drawerType = ScriptAttributeUtilityWrapper.GetDrawerTypeForType(attribute.GetType());
                if (drawerType != null) {
                    var drawer = (IAnySerializableAttributeEditor)Activator.CreateInstance(drawerType);
                    var methodGUI = drawer.CreateGUI(attribute, method, target);
                    if(methodGUI != null)
                        customInspector.Add(methodGUI);
                }
            }
            
            return customInspector;
        }
    }
}