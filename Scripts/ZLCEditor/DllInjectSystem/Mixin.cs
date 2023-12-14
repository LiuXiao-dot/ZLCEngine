using System;
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
    }
}