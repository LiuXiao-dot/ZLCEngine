using UnityEditor;
using UnityEngine;
using ZLCEngine.ConfigSystem;
using ZLCEngine.Inspector;
using FilePathAttribute = ZLCEngine.ConfigSystem.FilePathAttribute;
namespace ZLCEditor.ResSystem
{
    /// <summary>
    /// 资源管理的工具
    /// </summary>
    [Tool("资源")]
    [FilePath(FilePathAttribute.PathType.XWEditor, true)]
    public class ResTool : SOSingleton<ResTool>
    {
        /// <summary>
        /// 资源目录路径
        /// </summary>
        [Header("目录路径")]
        [FolderPath]
        public string[] dirs;

        /// <summary>
        /// 资源同步
        /// </summary>
        [Button("资源同步")]
        public void SyncAddressalbes()
        {
            EditorUtility.DisplayDialog("资源同步", ResHelper.Sync(dirs),"确认");
        }

        /// <summary>
        /// 构建项目
        /// </summary>
        [Button("Build")]
        public void BuildProject()
        {
            EditorUtility.DisplayDialog("Build", ResHelper.Build(dirs),"确认");
        }
    }
}