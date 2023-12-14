using System.IO;
using Mono.Cecil;
namespace ZLCEditor.DllInjectSystem
{
    /// <summary>
    ///     ModuleDefinition的扩展方法
    /// </summary>
    public static class ModuleDefinitionExtension
    {
        /// <summary>
        ///     读取dll模块
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static ModuleDefinition ReadModule(string fileName)
        {
            ReaderParameters readerParameters = new ReaderParameters();
            ZLCAssemblyResolver assemblyResolver = new ZLCAssemblyResolver();
            assemblyResolver.AddSearchDirectory(Path.GetDirectoryName(fileName));
            readerParameters.AssemblyResolver = assemblyResolver;
            return ModuleDefinition.ReadModule(fileName, readerParameters);
        }
    }
}