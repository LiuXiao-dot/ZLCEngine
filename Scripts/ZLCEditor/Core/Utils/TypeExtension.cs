using System;
using System.Collections.Generic;
namespace ZLCEditor.Utils
{
    public static class TypeExtension
    {
        public static IEnumerable<Type> GetBaseTypes(this Type type)
        {
            var parent = type.BaseType;
            while (parent != null) {
                yield return parent;
                parent = parent.BaseType;
            }
        }
    }
}