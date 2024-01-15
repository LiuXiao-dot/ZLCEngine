using UnityEngine;
using ZLCEngine.Inspector;
namespace ZLCEngine.TerrainSystem.Boolean.Controllers
{
    public class NoneShapeController : IShapeController
    {
        public static NoneShapeController Instance = new NoneShapeController();
        
        [ReadOnly]public string tip = "无控制器";
        public void CreateNewShape()
        {
        }
        public void RefreshMesh()
        {
        }
        public Mesh GetMesh()
        {
            return null;
        }
        public IShape GetShape()
        {
            return null;
        }
    }
}