using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using ZLCEngine.SerializeTypes;
namespace ZLCEditor.Inspector
{
    [CustomPropertyDrawer(typeof(SType))]
    public class STypeDrawer : PropertyDrawer
    {

        private static List<string> types;
        private int selected;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            DropdownField dropdownField = new DropdownField();
            dropdownField.choices = GetTypes();
            return dropdownField;
        }

        private List<string> GetTypes()
        {
            if (types != null) return types;
            List<Type> ts = new List<Type>();
            EditorApplication.LockReloadAssemblies();
            try {
                /*var assemblies = CompilationPipeline.GetAssemblies();
                foreach (var assembly in assemblies) {
                    var realAssembly = System.Reflection.Assembly.Load(assembly.name);
                    foreach (var type in realAssembly.GetTypes()) {
                        types.Add(type);
                    }
                }*/
                EditorHelper.GetAllChildType<object>(ts, EditorHelper.AssemblyFilterType.Internal);
                types = ts.Select(t => t.FullName).ToList();
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
        [Serializable]
        private struct TempType
        {
            public string className;
            public Type type;

            public static implicit operator TempType(Type type)
            {
                return new TempType
                {
                    type = type,
                    className = type.FullName
                };
            }
        }
    }
}