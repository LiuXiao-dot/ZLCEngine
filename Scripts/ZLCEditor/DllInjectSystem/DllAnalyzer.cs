using System;
using System.IO;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;
using UnityEngine;
using ZLCEditor.DllInjectSystem;
using ZLCEngine.ConfigSystem;
using ZLCEngine.Inspector;
using MethodAttributes = Mono.Cecil.MethodAttributes;
using TypeAttributes = Mono.Cecil.TypeAttributes;
namespace DllInjectSystem
{
    /// <summary>
    ///     Dll解析器
    /// </summary>
    [FilePath(FilePathAttribute.PathType.XWEditor, true)]
    [Tool("代码注入/程序集解析")]
    public class DllAnalyzer : SOSingleton<DllAnalyzer>
    {
        public string path;
        [Button]
        public void Test()
        {
            if (string.IsNullOrEmpty(path)) {
                path = "C:\\Users\\安然\\Desktop\\UnityDllModify\\UnityEditor.CoreModule.dll";
            }
            // 加载dll
            ReaderParameters readerParameters = new ReaderParameters();
            ZLCAssemblyResolver assemblyResolver = new ZLCAssemblyResolver();
            assemblyResolver.AddSearchDirectory(Path.GetDirectoryName(path));
            readerParameters.AssemblyResolver = assemblyResolver;
            ModuleDefinition moduleDefinition = ModuleDefinition.ReadModule(path, readerParameters);

            TypeDefinition testClass = new TypeDefinition("DllInjectSystem", "InjectSample", TypeAttributes.Class, moduleDefinition.ImportReference(typeof(object)));
            moduleDefinition.Types.Add(testClass);

            //准备数据
            moduleDefinition.AssemblyResolver.Resolve(AssemblyNameReference.Parse("UnityEngine.CoreModule"));
            moduleDefinition.ImportReference(typeof(Debug)).Resolve();
            MethodReference unityEngineDebugLog = moduleDefinition.ImportReference(typeof(Debug).GetMethod("Log", new[]
            {
                typeof(object)
            }));
            TypeReference voidType = moduleDefinition.ImportReference(typeof(void));
            MethodReference objectCtor = moduleDefinition.ImportReference(typeof(object).GetConstructor(Type.EmptyTypes));

            // 添加构造函数
            AddDefaultConstructor(testClass, objectCtor, moduleDefinition);

            MethodDefinition method = new MethodDefinition("Debug", MethodAttributes.Public, voidType);
            testClass.Methods.Add(method);
            method.Parameters.Add(new ParameterDefinition(moduleDefinition.ImportReference(typeof(string))));

            ILProcessor debugIL = method.Body.GetILProcessor();
            debugIL.Append(debugIL.Create(OpCodes.Nop));
            debugIL.Append(debugIL.Create(OpCodes.Ldarg_1)); // 加载第一个参数到堆栈上
            debugIL.Append(debugIL.Create(OpCodes.Call, unityEngineDebugLog));
            debugIL.Append(debugIL.Create(OpCodes.Nop));
            debugIL.Append(debugIL.Create(OpCodes.Ret));

            moduleDefinition.Write(Path.Combine(Path.GetDirectoryName(path), "Generator", Path.GetFileName(path)));
        }

        private void AddDefaultConstructor(TypeDefinition type, MethodReference baseEmptyConstructor, ModuleDefinition module)
        {
            MethodAttributes methodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;
            MethodDefinition method = new MethodDefinition(".ctor", methodAttributes, module.TypeSystem.Void);
            //var method = new MethodDefinition(".ctor", methodAttributes, TypeSystem.Void);
            method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            method.Body.Instructions.Add(Instruction.Create(OpCodes.Call, baseEmptyConstructor));
            method.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
            type.Methods.Add(method);
        }

        [Button]
        public void Log()
        {
            Assembly assembly = Assembly.LoadFile(Path.Combine(Path.GetDirectoryName(path), "Generator", Path.GetFileName(path)));
            Type dllInjectSystem = assembly.GetType("DllInjectSystem.InjectSample");
            object instance = Activator.CreateInstance(dllInjectSystem);
            MethodInfo method = dllInjectSystem.GetMethod("Debug", BindingFlags.Public | BindingFlags.Instance);
            method?.Invoke(instance, new object[]
            {
                "log success"
            });
        }

        /// <summary>
        ///     为DecoratorDrawer添加一个字段
        /// </summary>
        [Button]
        public void AddField2DecoratorDrawer()
        {

        }
    }

    /*
     .method public hidebysig specialname rtspecialname instance void
    .ctor() cil managed
  {
    .maxstack 8

    IL_0000: ldarg.0      // this
    IL_0001: call         instance void [netstandard]System.Object::.ctor()
    IL_0006: nop
    IL_0007: ret

  } // end of method InjectSample::.ctor
     
     .method public hidebysig instance void
        Debug(
            string 'value'
        ) cil managed
    {
        .maxstack 8

        // [38 9 - 38 10]
        IL_0000: nop

        // [39 13 - 39 42]
        IL_0001: ldarg.1      // 'value'
        IL_0002: call         void [UnityEngine.CoreModule]UnityEngine.Debug::Log(object)
        IL_0007: nop

        // [40 9 - 40 10]
        IL_0008: ret

    } // end of method InjectSample::Debug*/

    public class InjectSample
    {
        public void Debug(string value)
        {
            UnityEngine.Debug.Log(value);
        }
    }
}