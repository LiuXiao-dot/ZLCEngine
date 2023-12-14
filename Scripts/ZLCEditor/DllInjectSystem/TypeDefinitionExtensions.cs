using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using UnityEngine;
namespace ZLCEditor.DllInjectSystem
{
    /// <summary>
    ///     TypeDefinition扩展方法
    /// </summary>
    public static class TypeDefinitionExtensions
    {
        /// <summary>
        ///     添加默认构造函数
        /// </summary>
        /// <param name="type"></param>
        /// <param name="module"></param>
        public static void AddDefaultConstructor(this TypeDefinition type)
        {
            ModuleDefinition module = type.Module;
            MethodReference objectCtor = module.ImportReference(typeof(object).GetConstructor(Type.EmptyTypes));
            MethodAttributes methodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;

            MethodDefinition method = new MethodDefinition(".ctor", methodAttributes, module.TypeSystem.Void);
            method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            method.Body.Instructions.Add(Instruction.Create(OpCodes.Call, objectCtor));
            method.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
            type.Methods.Add(method);
        }

        /// <summary>
        ///     添加字段
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="attributes"></param>
        /// <param name="fieldType"></param>
        public static void AddField(this TypeDefinition type, string name, FieldAttributes attributes, Type fieldType)
        {
            ModuleDefinition module = type.Module;
            TypeReference fieldReference = module.ImportReference(fieldType);
            FieldDefinition filed = new FieldDefinition(name, attributes, fieldReference);
            if (type.Fields.Any(t => t.Name == name)) {
                Debug.LogError($"{type.Name}中已经存在Filed{name}，无法再添加");
                return;
            }

            type.Fields.Add(filed);
        }

        /// <summary>
        ///     添加初始化的字段
        /// </summary>
        public static void AddDefaultInitField()
        {

        }
    }
}