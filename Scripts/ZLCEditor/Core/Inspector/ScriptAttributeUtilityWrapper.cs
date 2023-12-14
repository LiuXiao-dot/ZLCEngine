using System;
using System.Reflection;
using UnityEditor;
namespace ZLCEditor.Inspector
{
    public class ScriptAttributeUtilityWrapper
    {
        private static MethodInfo _getDrawerTypeForType;
        static ScriptAttributeUtilityWrapper()
        {
            Type realType = typeof(CustomPropertyDrawer).Assembly.GetType("UnityEditor.ScriptAttributeUtility");
            _getDrawerTypeForType = realType.GetMethod("GetDrawerTypeForType", BindingFlags.NonPublic | BindingFlags.Static);
        }

        public static Type GetDrawerTypeForType(Type type)
        {
            return (Type)_getDrawerTypeForType?.Invoke(null, new object[]
            {
                type
            });
        }
    }
}