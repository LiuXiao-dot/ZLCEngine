using System;
using Mono.Cecil;
using Mono.Collections.Generic;
namespace ZLCEditor.DllInjectSystem
{
    public static class Mixin
    {
        public enum Argument
        {
            name,
            fileName,
            fullName,
            stream,
            type,
            method,
            field,
            parameters,
            module,
            modifierType,
            eventType,
            fieldType,
            declaringType,
            returnType,
            propertyType,
            interfaceType,
            constraintType
        }

        public static Version ZeroVersion = new Version(0, 0, 0, 0);


        public static void CheckName(object name)
        {
            if (name == null)
                throw new ArgumentNullException(Argument.name.ToString());
        }

        public static T[] Resize<T>(this T[] self, int length)
        {
            Array.Resize(ref self, length);
            return self;
        }

        public static void CheckParameters(object parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException(Argument.parameters.ToString());
        }
        
        
        public static bool IsSameParameters(Collection<ParameterDefinition> a, Collection<ParameterDefinition> b)
        {
            if (a == null && b == null) return true;
            if (a == null || b == null) return false;
            if (a.Count != b.Count) return false;
            var length = a.Count;
            for (int i = 0; i < length; i++) {
                if (a[i].ParameterType.FullName != b[i].ParameterType.FullName) return false;
            }
            return true;
        }
    }
}