using System;
using ZLCEditor.Inspector;
using ZLCEngine.ConfigSystem;
namespace ZLCEditor.Tool
{
    /// <summary>
    /// 条件编译工具
    /// </summary>
    [Tool("配置/条件编译")]
    [Serializable]
    public class ScriptDefineTool
    {
        public ScriptDefine zlcDebug = new ScriptDefine()
        {
            name = "ZLC_DEBUG"
        };
    }
}