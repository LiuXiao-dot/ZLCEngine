using System;
using System.Collections.Generic;
using UnityEngine;
using ZLCEngine.Inspector;
namespace ZLCEngine.TerrainSystem.Boolean
{
    /// <summary>
    /// 形状层级
    /// </summary>
    [Serializable]
    public class ShapeLayer
    {
        public int layer;
        
        [SerializeReference]
        [VirtualSerialize]
        public List<IShape> shapes = new List<IShape>();
    }
}