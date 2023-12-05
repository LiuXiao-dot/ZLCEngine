using System;
using ZLCEngine.SerializeTypes;
namespace ZLCEditor.ZLCCoding
{
    /// <summary>
    /// ZLC格式的文本
    /// </summary>
    [Serializable]
    public struct ZLCCode
    {
        /// <summary>
        /// 实际的代码
        /// </summary>
        public string code;
        /// <summary>
        /// 输入到代码中的参数
        /// </summary>
        public SDictionary<string, object> kvs;
    }
}