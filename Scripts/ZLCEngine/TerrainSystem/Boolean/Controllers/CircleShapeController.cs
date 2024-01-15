using System;
using UnityEngine;
using ZLCEngine.Inspector;
using ZLCEngine.TerrainSystem.Boolean.Shapes;
namespace ZLCEngine.TerrainSystem.Boolean.Controllers
{
    /// <summary>
    /// 圆形控制器
    /// </summary>
    [Serializable]
    public class CircleShapeController : AShapeController
    {
        [ReadOnly]public string tip = "圆形";
        public override void CreateNewShape()
        {
            this.shape = new CircleShape()
            {
                radius = 0.5f,
                position = Vector2.zero,
                segments = 12
            };
            this.mesh = this.shape.CreateMesh();
        }
    }
}