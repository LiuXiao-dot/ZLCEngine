using ZLCEngine.ConfigSystem;
using ZLCEngine.SerializeTypes;
namespace ZLCEngine.SaveSystem
{
    /// <summary>
    /// 自定义的解析器
    /// </summary>
    [FilePath(FilePathAttribute.PathType.XW, true)]
    [Tool("存档")]
    public class CustomResolverSO : SOSingleton<CustomResolverSO>
    {
        public SType[] resolveTypes;

        #if UNITY_EDITOR
        /// <summary>
        /// 编辑器下的存档,运行时都要从存档位置获取
        /// </summary>
        public SDictionary<string, string> saved;
        #endif
    }
}