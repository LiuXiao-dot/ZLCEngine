using System;
using System.Reflection;
using UnityEditor;
namespace ZLCEditor.Inspector
{
    public class CustomEditorAttributesWrap
    {
        private static MethodInfo _findCustomEditorType;
        static CustomEditorAttributesWrap()
        {
            Type realType = typeof(CustomPropertyDrawer).Assembly.GetType("UnityEditor.CustomEditorAttributes");
            _findCustomEditorType = realType.GetMethod("FindCustomEditorTypeByType", BindingFlags.NonPublic | BindingFlags.Static);
        }

        public static Type FindCustomEditorTypeByType(Type type, bool multiEdit)
        {
            return (Type)_findCustomEditorType?.Invoke(null, new object[]
            {
                type, multiEdit
            });
        }
    }
}