using Mono.Cecil;
namespace ZLCEditor.DllInjectSystem
{
    public static class FieldDefinitionExtension
    {
        public static void ChangeFieldAttributes(this FieldDefinition fieldDefinition, FieldAttributes fieldAttributes)
        {
            fieldDefinition.Attributes = fieldAttributes;
        }
    }
}