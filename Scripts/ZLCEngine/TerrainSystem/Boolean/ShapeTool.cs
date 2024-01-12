#if UNITY_EDITOR
using System;
using UnityEngine.UIElements;
using ZLCEngine.SerializeTypes;
namespace ZLCEngine.TerrainSystem.Boolean
{
    /// <summary>
    /// 形状工具
    /// </summary>
    [Serializable]
    public class ShapeTool
    {
        /// <summary>
        /// 形状类型
        /// </summary>
        public SType shapeType;
        /// <summary>
        /// 图标
        /// </summary>
        public VectorImage icon;
    }
}
#endif
