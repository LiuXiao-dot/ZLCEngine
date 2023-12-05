using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
namespace ZLCEditor.Tool
{
    [ZLCEngine.ConfigSystem.Tool("动态代码运行")]
    public class DynamicTool
    {
        public AssemblyDefinitionAsset[] AssemblyDefinitionAssets;
        [AssetList(CustomFilterMethod = "CheckName")]
        public DefaultAsset[] AssemblyReferences;
        
        /// <summary>
        /// 检测后缀为dll的文件
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        private bool CheckName(DefaultAsset asset)
        {
            return Path.GetExtension(AssetDatabase.GetAssetPath(asset)) == ".dll";
        }
        
        [LabelText("代码")]
        [TextArea(minLines:10, maxLines:50)]
        public string code;
        
        [Button("运行")]
        public void Run()
        {
            if(string.IsNullOrEmpty(code)) return;
            var completeCode = @$"
using UnityEngine;
public class JIT
{{
    public static void Run()
    {{
        {code}
    }}
}}";
            var syntaxTree = ParseToSyntaxTree(completeCode);
            var compilation = BuildCompilation(syntaxTree);
            var assembly = ComplieToAssembly(compilation);
            var jitType = assembly.GetType("JIT");
            var method = jitType.GetMethod("Run", BindingFlags.Public | BindingFlags.Static);
            method.Invoke(null, null);
        }

        public SyntaxTree ParseToSyntaxTree(string code)
        {
            var parseOptions = new CSharpParseOptions(LanguageVersion.Latest, preprocessorSymbols: new[] { "RELEASE" });
            // 有许多其他配置项，最简单这些就可以了
            return CSharpSyntaxTree.ParseText(code, parseOptions);
        }

        public CSharpCompilation BuildCompilation(SyntaxTree syntaxTree)
        {
            var compilationOptions = new CSharpCompilationOptions(
                concurrentBuild: true,
                metadataImportOptions: MetadataImportOptions.All,
                outputKind: OutputKind.DynamicallyLinkedLibrary,
                optimizationLevel: OptimizationLevel.Release,
                allowUnsafe: true,
                platform: Platform.AnyCpu,
                checkOverflow: false,
                assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default);
            // 有许多其他配置项，最简单这些就可以了
            var references = typeof(DynamicTool).Assembly.GetReferencedAssemblies().Select(name => Assembly.Load(name.Name))
                .Distinct()
                .Select(i => MetadataReference.CreateFromFile(i.Location));
            references = references.Append(MetadataReference.CreateFromFile(Assembly.Load("netstandard").Location));
            references = references.Append(MetadataReference.CreateFromFile(Assembly.Load("UnityEngine.CoreModule").Location));
            // 获取编译时所需用到的dll， 这里我们直接简单一点 copy 当前执行环境的
            return CSharpCompilation.Create("JIT.cs", new SyntaxTree[] { syntaxTree }, references, compilationOptions);
        }

        public Assembly ComplieToAssembly(CSharpCompilation compilation)
        {
            using (var stream = new MemoryStream())
            {
                var restult = compilation.Emit(stream);
                if (restult.Success)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    return Assembly.Load(stream.ToArray());
                }
                else
                {
                    throw new Exception(restult.Diagnostics.Select(i => i.ToString()).DefaultIfEmpty().Aggregate((i, j) => i + j));
                }
            }
        }

    }
}