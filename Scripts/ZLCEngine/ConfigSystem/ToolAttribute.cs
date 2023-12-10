using System;
namespace ZLCEngine.ConfigSystem
{
    /// <summary>
    ///     使用该Attribute标记的类会被添加到ToolConfig中
    /// </summary>
    public class ToolAttribute : Attribute
    {
        /// <summary>
        ///     是否检测子工具
        ///     如果是ScriptableObject，则获取SubAsset,并调用GetSubTools()方法
        ///     如果不是，则仅调用GetSubTools()方法
        ///     todo:支持GetSubTools()方法
        /// </summary>
        public bool checkSub;
        public string path;

        /// <summary>
        /// </summary>
        /// <param name="path">工具的路径</param>
        /// <param name="checkSub">是否检测子工具</param>
        public ToolAttribute(string path = null, bool checkSub = false)
        {
            this.path = path;
            this.checkSub = true;
        }
    }
}