using UnityEngine;
namespace ZLCEngine.TerrainSystem.Boolean.Controllers
{
    /// <summary>
    /// 形状控制器抽象类
    /// </summary>
    public abstract class AShapeController : IShapeController
    {
        public IShape shape;
        public Mesh mesh;

        /// <summary>
        /// 创建新的形状
        /// </summary>
        public abstract void CreateNewShape();
        public void RefreshMesh()
        {
            mesh = shape.CreateMesh();
        }
        public Mesh GetMesh()
        {
            return mesh;
        }
        public IShape GetShape()
        {
            return shape;
        }
    }
}