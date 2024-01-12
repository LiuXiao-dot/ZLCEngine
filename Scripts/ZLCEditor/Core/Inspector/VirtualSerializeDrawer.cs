using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using ZLCEditor.Inspector.Menu;
using ZLCEngine.Inspector;
namespace ZLCEditor.Inspector
{
    [CustomPropertyDrawer(typeof(VirtualSerializeAttribute))]
    public class VirtualSerializeDrawer : PropertyDrawer
    {
        private static List<string> types;
        private static List<Type> realTypes;
        private static VectorImage icon;
        private const string iconUrl = "Packages/com.zlc.zlcengine/Assets/svgs/board_magic_wand.svg";
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            // 基本UI元素不变，添加一个选择按钮，点击后可以选择创建的实例
            var root = new VisualElement();
            var baseElement = ZLCDrawerHelper.CreateDrawer(property);
            baseElement.name = "base-element";
            if (icon == null) {
                icon = AssetDatabase.LoadAssetAtPath<VectorImage>(iconUrl);
            }
            
            var choices = GetTypes();
            var defaultIndex = choices.IndexOf(fieldInfo.FieldType.FullName);

            root.Add(SearchMenu.CreateButton(types, newIndex =>
            {
                var newType = realTypes[newIndex];
                var newInstance = Activator.CreateInstance(newType);
                property.boxedValue = newInstance;
                property.serializedObject.ApplyModifiedProperties();
                baseElement.Bind(property.serializedObject);
            }, defaultIndex, icon, false));
            root.Add(baseElement);
            return root;
        }
        
        private List<string> GetTypes()
        {
            if (types != null) return types;
            List<Type> ts = new List<Type>();
            realTypes = ts;
            EditorApplication.LockReloadAssemblies();
            try {
                var virtualType = fieldInfo.FieldType;
                if (virtualType.IsArrayOrList()) {
                    virtualType = virtualType.GenericTypeArguments[0];
                }
                EditorHelper.GetAllChildType(ts, EditorHelper.AssemblyFilterType.Internal | EditorHelper.AssemblyFilterType.Custom, virtualType);
                ts.RemoveAll(t => t.IsAbstract || t.IsInterface);
                types = ts.Distinct().Select(t => t.FullName).ToList();
            }
            catch (Exception e) {
                Debug.LogError(e);
                throw;
            }
            finally {
                EditorApplication.UnlockReloadAssemblies();
            }
            return types;
        } 
    }
}