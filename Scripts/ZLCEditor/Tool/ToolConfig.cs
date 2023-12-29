using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Compilation;
using ZLCEngine.ConfigSystem;
using ZLCEngine.Inspector;
using ZLCEngine.SerializeTypes;
using ZLCEngine.Utils;
using Assembly = System.Reflection.Assembly;
using FilePathAttribute = ZLCEngine.ConfigSystem.FilePathAttribute;
namespace ZLCEditor.Tool
{
    /// <summary>
    ///     工具配置
    ///     用于保存所有工具的类型信息，减少重新查找的次数。
    ///     为此需要知道是否需要重新查找。
    /// </summary>
    [FilePath(FilePathAttribute.PathType.XWEditor, true)]
    [Serializable]
    [Tool("配置/工具")]
    internal class ToolConfig : SOSingleton<ToolConfig>
    {
        private static List<Type> _typesPool;

        [ReadOnly]
        public SDictionary<string, SType[]> toolTypes;

        /// <summary>
        ///     开始监控是否又程序集发生变动，如果有，将这个程序集的相关类型清空并且重新保存。
        /// </summary>
        [InitializeOnLoadMethod]
        private static void Monitor()
        {
            CompilationPipeline.compilationStarted += OnCompilzationStarted;
            CompilationPipeline.compilationFinished += OnCompilzationFinished;
            CompilationPipeline.assemblyCompilationFinished += OnAssemblyCompilationFinished;
        }

        /// <summary>
        ///     如果该程序集引用了ZLCEditor.Tool程序集，才生效
        /// </summary>
        /// <param name="url">程序集路径</param>
        /// <param name="messages">编译器消息</param>
        private static void OnAssemblyCompilationFinished(string url, CompilerMessage[] messages)
        {
            if (Instance == null) return;
            SDictionary<string, SType[]> toolTypes = Instance.toolTypes == null ? new SDictionary<string, SType[]>() : Instance.toolTypes;
            
            Assembly assembly = Assembly.Load(Path.GetFileNameWithoutExtension(url));
            Instance.RefreshAssembly(assembly);
        }

        private static void OnCompilzationFinished(object context)
        {
            _typesPool = null;
        }


        private static void OnCompilzationStarted(object context)
        {
            _typesPool = new List<Type>();
        }

        private void RefreshAssembly(Assembly assembly)
        {
            _typesPool ??= new List<Type>();
            AssemblyName[] referencedAssemblies = assembly.GetReferencedAssemblies();
            if (referencedAssemblies.Any(t => t.Name == Constant.AssemblyName)) {
                // 查找所有使用ToolAttribute标记的类，并缓存下来
                List<SType> resultList = new List<SType>();
                AssemblyHelper.GetAttributedTypes<ToolAttribute>(assembly, _typesPool, true);
                foreach (Type typePool in _typesPool) {
                    resultList.Add(typePool);
                }
                if (toolTypes.ContainsKey(assembly.FullName)) {
                    toolTypes[assembly.FullName] = resultList.ToArray();
                } else {
                    toolTypes.Add(assembly.FullName, resultList.ToArray());
                }

                _typesPool.Clear();
            } else {
                // 如果Instance.toolTypes里包含这个程序集的工具，则剔除
                if (Instance.toolTypes.ContainsKey(assembly.FullName)) {
                    Instance.toolTypes.Remove(assembly.FullName);
                }
            }
            Instance.toolTypes = toolTypes;
            EditorUtility.SetDirty(Instance);
            AssetDatabase.SaveAssetIfDirty(Instance);
        }

        /// <summary>
        ///     刷新全部程序集内容
        /// </summary>
        [Button]
        internal void Refresh()
        {
            if (EditorUtility.DisplayDialog("ToolConfig", "此操作将会重新检查全部程序集中包含的工具内容，可能需要较长时间，是否继续？", "是")) {
                IEnumerable<Assembly> assemblys = CompilationPipeline.GetAssemblies().Select(t =>
                    Assembly.Load(Path.GetFileNameWithoutExtension(t.outputPath)));
                foreach (Assembly assembly in assemblys) {
                    RefreshAssembly(assembly);
                }
                EditorUtility.SetDirty(this);
            }
        }
    }
}