using Mono.Cecil;
namespace ZLCEditor.DllInjectSystem
{
    public static class MethodDefinitionExtension
    {
        public static void ChangeMethodAttributes(this MethodDefinition methodDefinition, MethodAttributes methodAttributes)
        {
            methodDefinition.Attributes = methodAttributes;
        }
    }
}