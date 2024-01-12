using UnityEditor;
using UnityEditor.EditorTools;
namespace ZLCEditor.TerrainSystem.Boolean.SceneTools
{
    /// <summary>
    /// 操纵工具
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal abstract class ManipulationTool<T> : EditorTool where T : EditorTool
    {
        public override void OnToolGUI(EditorWindow window)
        {
            var view = window as TerrainView;
            if(!view) return;
            
            // 将会操纵当前选择的形状控制器
            
            
        }
    }
}