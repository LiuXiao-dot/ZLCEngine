using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using ZLCEditor.Inspector.Menu;
using ZLCEngine.SerializeTypes;
namespace ZLCEditor.Inspector
{
    [CustomPropertyDrawer(typeof(SType))]
    public class STypeInfoDrawer : PropertyDrawer
    {
        private static List<string> types;
        private static List<Type> realTypes;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var choices = GetTypes();
            var defaultIndex = choices.IndexOf(property.structValue.ToString());
            var typeProperty = property.FindPropertyRelative("typeName");
            var assemblyProperty = property.FindPropertyRelative("assemblyName");
            defaultIndex = defaultIndex == -1 ? 0 : defaultIndex;

            return SearchMenu.CreateDropdownField(choices, (newIndex, s) =>
            {
                var newType = realTypes[newIndex];
                typeProperty.stringValue = newType.FullName;
                assemblyProperty.stringValue = newType.Assembly.FullName;
                property.serializedObject.ApplyModifiedProperties();
            }, defaultIndex);
        }

        private List<string> GetTypes()
        {
            if (types != null) return types;
            List<Type> ts = new List<Type>();
            realTypes = ts;
            EditorApplication.LockReloadAssemblies();
            try {
                EditorHelper.GetAllChildType<object>(ts, EditorHelper.AssemblyFilterType.Internal | EditorHelper.AssemblyFilterType.Custom);
                EditorHelper.GetAllInterfaces(ts, EditorHelper.AssemblyFilterType.Internal | EditorHelper.AssemblyFilterType.Custom);
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