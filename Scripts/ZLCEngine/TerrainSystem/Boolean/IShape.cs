using UnityEngine;
namespace ZLCEngine.TerrainSystem.Boolean
{
    /// <summary>
    /// 形状接口
    /// 形状是网格的基础数据，网格由一个个形状组合而成
    /// </summary>
    public interface IShape
    {
        Mesh CreateMesh();
    }

    /// <summary>
    /// 形状基类
    /// </summary>
    public abstract class AShape : IShape
    {
        /// <summary>
        /// 层级，按照层级关系对形状进行混合
        /// </summary>
        public int layer;
        
        public abstract Mesh CreateMesh();
    }
}