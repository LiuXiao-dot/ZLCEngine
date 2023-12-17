using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Collections.Generic;
using UnityEngine;
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

        public static void ChangeMethodsToVirtualAndPublic(this ModuleDefinition moduleDefinition, string typeName, string[] methodNames, Collection<ParameterDefinition>[] parameterDefinitions = null)
        {
            var length = methodNames.Length;
            for (int i = 0; i < length; i++) {
                ChangeMethodToVirtualAndPublic(moduleDefinition, typeName, methodNames[i], parameterDefinitions[i]);
            }
        }

        public static void ChangeMethodToVirtualAndPublic(this ModuleDefinition moduleDefinition, string typeName, string methodName, Collection<ParameterDefinition> parameterDefinitions = null)
        {
            TypeDefinition type = moduleDefinition.GetType(typeName);
            IEnumerable<MethodDefinition> methods = type.Methods.Where(t => t.Name == methodName && Mixin.IsSameParameters(t.Parameters, parameterDefinitions));
            if (methods.Any()) {
                MethodDefinition method = methods.First();
                method.ChangeMethodAttributes(MethodAttributes.Virtual | MethodAttributes.Public);
            } else {
                Debug.LogError($"未找到方法{typeName}.{methodName}");
            }
        }

        public static void ChangeTypeToPublic(this ModuleDefinition moduleDefinition, string typeName)
        {
            TypeDefinition type = moduleDefinition.GetType(typeName);
            type.Attributes = type.Attributes & ~TypeAttributes.NotPublic | TypeAttributes.Public;
        }

        public static void ChangeFieldsToPublic(this ModuleDefinition moduleDefinition, string typeName, string[] fieldNames)
        {
            foreach (var filedName in fieldNames) {
                ChangeFieldToPublic(moduleDefinition, typeName, filedName);
            }
        }

        public static void ChangeFieldToPublic(this ModuleDefinition moduleDefinition, string typeName, string fieldName)
        {
            TypeDefinition type = moduleDefinition.GetType(typeName);
            IEnumerable<FieldDefinition> fields = type.Fields.Where(t => t.Name == fieldName);
            if (fields.Any()) {
                FieldDefinition field = fields.First();
                field.ChangeFieldAttributes(FieldAttributes.Public);
            } else {
                Debug.LogError($"未找到字段{typeName}.{fieldName}");
            }
        }
    }
}