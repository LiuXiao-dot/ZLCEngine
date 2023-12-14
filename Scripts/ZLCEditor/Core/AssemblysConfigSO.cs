using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using ZLCEngine.ConfigSystem;
using ZLCEngine.Inspector;
using ZLCEngine.Utils;
using FilePathAttribute = ZLCEngine.ConfigSystem.FilePathAttribute;
namespace ZLCEditor
{
    [Tool("配置/程序集")]
    [ZLCEngine.ConfigSystem.FilePath(FilePathAttribute.PathType.XWEditor, true)]
    public sealed class AssemblysConfigSO : SOSingleton<AssemblysConfigSO>
    {
        [BoxGroup("ZLC框架提供的程序集")]
        [ReadOnly]
        public List<AssemblyDefinitionAsset> defaultAssemblies;

        [BoxGroup("ZLC框架提供的程序集")]
        [AssetList(CustomFilterMethod = "CheckName")]
        public List<DefaultAsset> defaultDlls;

        [Tooltip("许多工具都不会扫描官方或第三方程序集，只扫描自定义程序集。")]
        [BoxGroup("自定义程序集(需要手动添加)")]
        public List<AssemblyDefinitionAsset> selfAssemblies;

        [BoxGroup("自定义程序集(需要手动添加)")]
        [AssetList(CustomFilterMethod = "CheckName")]
        public List<DefaultAsset> selfDlls;

        [BoxGroup("unity官方的程序集")]
        public List<AssemblyDefinitionAsset> unityAssemblies;

        [BoxGroup("unity官方的程序集")]
        [AssetList(CustomFilterMethod = "CheckName")]
        public List<DefaultAsset> unityDlls;

        [Tooltip("(自动刷新，会添加Assets目录下Plugins目录的程序集)")]
        [BoxGroup("第三方程序集")]
        public List<AssemblyDefinitionAsset> otherAssemblies;

        [Tooltip("(自动刷新，会添加Assets目录下Plugins目录的程序集)")]
        [BoxGroup("第三方程序集")]
        [AssetList(CustomFilterMethod = "CheckName")]
        public List<DefaultAsset> otherDlls;

        /// <summary>
        ///     重置时调用一次刷新程序集
        /// </summary>
        private void Reset()
        {
            RefreshAssemblies();
        }

        /// <summary>
        ///     检测后缀为dll的文件
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        private bool CheckName(DefaultAsset asset)
        {
            return Path.GetExtension(AssetDatabase.GetAssetPath(asset)) == ".dll";
        }

        [Button("刷新程序集")]
        public void RefreshAssemblies()
        {
            // 刷新ZLC提供的程序集
            string zlcAssembliesPromot = "Scripts*ZLC*.asmdef";
            string zlcDllsPromot = "Scripts*ZLC*.dll";
            EditorHelper.SearchPackagesAssets<AssemblyDefinitionAsset>(zlcAssembliesPromot, temp =>
            {
                defaultAssemblies = temp.ToList();
            });
            EditorHelper.SearchPackagesAssets<DefaultAsset>(zlcDllsPromot, temp =>
            {
                defaultDlls = temp.ToList();
            });

            // 不处理自定义程序集，如果有空值则移除
            IListHelper.RemoveNulls(selfAssemblies);
            IListHelper.RemoveNulls(selfDlls);

            // 刷新Unity自带的程序集
            // Core,UnityEditor.UI,UnityEngine.UI
            string tempAssembliesPromot = "UnityEngine*.asmdef or Unity*.asmdef or UnityEditor*.asmdef";
            string tempDllsPromot = "Unity*.dll or UnityEngine*.dll or UnityEditor*.dll";
            EditorHelper.SearchPackagesAssets<AssemblyDefinitionAsset>(tempAssembliesPromot, temp =>
            {
                unityAssemblies = temp.ToList();
            });
            EditorHelper.SearchPackagesAssets<DefaultAsset>(tempDllsPromot, temp =>
            {
                unityDlls = temp.ToList();
            });
            // 不处理自定义程序集，如果有空值则移除
            IListHelper.RemoveNulls(unityAssemblies);
            IListHelper.RemoveNulls(unityDlls);

            // 刷新第三方程序集
            string pluginAssembliesPromot = "dir=Plugins ext:asmdef";
            string pluginDllsPromot = "dir=Plugins ext:dll";
            EditorHelper.SearchPackagesAssets<AssemblyDefinitionAsset>(pluginAssembliesPromot, temp =>
            {
                foreach (AssemblyDefinitionAsset defaultAssembly in defaultAssemblies) {
                    temp.Remove(defaultAssembly);
                }
                otherAssemblies = temp.ToList();
            });
            EditorHelper.SearchPackagesAssets<DefaultAsset>(pluginDllsPromot, temp =>
            {
                foreach (DefaultAsset defaultDll in defaultDlls) {
                    temp.Remove(defaultDll);
                }
                otherDlls = temp.ToList();
            });
            IListHelper.RemoveNulls(otherAssemblies);
            IListHelper.RemoveNulls(otherDlls);
        }
    }
}