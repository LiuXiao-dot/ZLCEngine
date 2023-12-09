using UnityEngine;
using ZLCEngine.ConfigSystem;
using FilePathAttribute = ZLCEngine.ConfigSystem.FilePathAttribute;
namespace ZLCEngine.ApplicationSystem
{
    /// <summary>
    /// 游戏配置数据
    /// </summary>
    [FilePath(FilePathAttribute.PathType.XW, true)]
    [Tool("配置")]
    public class AppConfigSO : SOSingleton<AppConfigSO>
    {
        /// <summary>
        /// UI场景的名字
        /// </summary>
        [Header("场景")]
        public string uiSceneName;
        /// <summary>
        /// 游戏场景的名字
        /// </summary>
        public string gameSceneName;

        public int firstMainWindowID;

        public int firstLoadingWindowID;

        /// <summary>
        /// 是否开启默认调试功能
        /// </summary>
        //[Header("调试")]
        //public LogMode logMode;
    }
}
