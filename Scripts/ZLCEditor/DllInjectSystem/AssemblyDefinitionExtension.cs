using System.Reflection;
using System.Runtime.CompilerServices;
using Mono.Cecil;
namespace ZLCEditor.DllInjectSystem
{
    public static class AssemblyDefinitionExtension
    {
        /// <summary>
        ///     添加友元程序集
        /// </summary>
        public static void AddVisibleToAssembly(this AssemblyDefinition assemblyDefinition, string assemblyName)
        {
            ModuleDefinition module = assemblyDefinition.MainModule;
            TypeReference stringReference = module.ImportReference(typeof(string));
            MethodBase method = typeof(InternalsVisibleToAttribute).GetConstructor(new[]
            {
                typeof(string)
            });
            CustomAttribute internalsVisibleTo = new CustomAttribute(module.ImportReference(method));
            internalsVisibleTo.ConstructorArguments.Add(new CustomAttributeArgument(stringReference, assemblyName));
            module.Assembly.CustomAttributes.Add(internalsVisibleTo);
        }
    }
}