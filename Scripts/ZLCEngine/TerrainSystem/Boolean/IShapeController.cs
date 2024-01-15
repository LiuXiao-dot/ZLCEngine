using UnityEngine;
namespace ZLCEngine.TerrainSystem.Boolean
{
    /// <summary>
    /// 形状控制器
    /// </summary>
    public interface IShapeController
    {
        void CreateNewShape();
        void RefreshMesh();
        Mesh GetMesh();
        IShape GetShape();
    }
}