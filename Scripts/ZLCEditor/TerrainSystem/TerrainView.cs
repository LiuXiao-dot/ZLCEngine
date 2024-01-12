using UnityEditor;
using UnityEditor.Overlays;
namespace ZLCEditor.TerrainSystem
{
    /// <summary>
    /// 地形编辑窗口
    /// </summary>
    [EditorWindowTitle(title = "ZLC-Terrain", useTypeNameAsIconName = true)]
    public class TerrainView : SceneView, ISupportsOverlays,IHasCustomMenu
    {
        /// <summary>
        /// 最近使用的地形编辑窗口
        /// </summary>
        internal static TerrainView lastActiveView;

        /// <summary>
        /// 展示地形编辑窗口
        /// </summary>
        internal static void Open()
        {
            if (lastActiveView != null) {
                lastActiveView.Focus();
                return;
            }
            //如果最后的场景不为空，则把地形窗口放到这里
            var view = EditorWindow.CreateWindow<TerrainView>(typeof(SceneView));
            TerrainView.lastActiveView = view;
        }
        
        public override void AddItemsToMenu(GenericMenu menu)
        {
           // menu.AddItem(new GUIContent("Hello"), false, OnHello);
        }
    }
}