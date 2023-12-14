using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using ZLCEngine.ConfigSystem;
using ZLCEngine.Inspector;
using FilePathAttribute = ZLCEngine.ConfigSystem.FilePathAttribute;
namespace ZLCEditor.Tool
{
    [Tool("动态代码运行")]
    [ZLCEngine.ConfigSystem.FilePath(FilePathAttribute.PathType.XWEditor, true)]
    public class DynamicTool : SOSingleton<DynamicTool>
    {
        public AssemblyDefinitionAsset[] AssemblyDefinitionAssets;
        [AssetList(CustomFilterMethod = "CheckName")]
        public DefaultAsset[] AssemblyReferences;

        [Header("代码")]
        [TextArea(10, 50)]
        public string code;

        /// <summary>
        ///     检测后缀为dll的文件
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        private bool CheckName(DefaultAsset asset)
        {
            return Path.GetExtension(AssetDatabase.GetAssetPath(asset)) == ".dll";
        }

        [Button("运行")]
        public void Run()
        {
            if (string.IsNullOrEmpty(code)) return;
            string completeCode = @$"
using UnityEngine;
public class JIT
{{
    public static void Run()
    {{
        {code}
    }}
}}";
            SyntaxTree syntaxTree = ParseToSyntaxTree(completeCode);
            CSharpCompilation compilation = BuildCompilation(syntaxTree);
            Assembly assembly = ComplieToAssembly(compilation);
            Type jitType = assembly.GetType("JIT");
            MethodInfo method = jitType.GetMethod("Run", BindingFlags.Public | BindingFlags.Static);
            method.Invoke(null, null);
        }

        public SyntaxTree ParseToSyntaxTree(string code)
        {
            CSharpParseOptions parseOptions = new CSharpParseOptions(LanguageVersion.Latest, preprocessorSymbols: new[]
            {
                "RELEASE"
            });
            // 有许多其他配置项，最简单这些就可以了
            return CSharpSyntaxTree.ParseText(code, parseOptions);
        }

        public CSharpCompilation BuildCompilation(SyntaxTree syntaxTree)
        {
            CSharpCompilationOptions compilationOptions = new CSharpCompilationOptions(
                concurrentBuild: true,
                metadataImportOptions: MetadataImportOptions.All,
                outputKind: OutputKind.DynamicallyLinkedLibrary,
                optimizationLevel: OptimizationLevel.Release,
                allowUnsafe: true,
                platform: Platform.AnyCpu,
                checkOverflow: false,
                assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default);
            // 有许多其他配置项，最简单这些就可以了
            IEnumerable<PortableExecutableReference> references = typeof(DynamicTool).Assembly.GetReferencedAssemblies().Select(name => Assembly.Load(name.Name))
                .Distinct()
                .Select(i => MetadataReference.CreateFromFile(i.Location));
            references = references.Append(MetadataReference.CreateFromFile(Assembly.Load("netstandard").Location));
            references = references.Append(MetadataReference.CreateFromFile(Assembly.Load("UnityEngine.CoreModule").Location));
            // 获取编译时所需用到的dll， 这里我们直接简单一点 copy 当前执行环境的
            return CSharpCompilation.Create("JIT.cs", new[]
            {
                syntaxTree
            }, references, compilationOptions);
        }

        public Assembly ComplieToAssembly(CSharpCompilation compilation)
        {
            using (MemoryStream stream = new MemoryStream()) {
                EmitResult restult = compilation.Emit(stream);
                if (restult.Success) {
                    stream.Seek(0, SeekOrigin.Begin);
                    return Assembly.Load(stream.ToArray());
                }
                throw new Exception(restult.Diagnostics.Select(i => i.ToString()).DefaultIfEmpty().Aggregate((i, j) => i + j));
            }
        }
    }
}