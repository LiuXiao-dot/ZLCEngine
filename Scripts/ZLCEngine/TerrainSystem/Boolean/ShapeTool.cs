using System;
using UnityEngine;
using UnityEngine.UIElements;
using ZLCEngine.Inspector;
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

        /// <summary>
        /// 控制器
        /// </summary>
        [VirtualSerialize]
        [SerializeReference]
        public IShapeController controller;
    }
}