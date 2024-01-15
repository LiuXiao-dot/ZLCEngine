using UnityEngine;
using ZLCEngine.TerrainSystem.Boolean;
using ZLCEngine.TerrainSystem.Boolean.Controllers;
namespace ZLCEditor.TerrainSystem.Boolean
{
    /// <summary>
    /// partial:这部分是UI以外的代码
    /// </summary>
    public partial class BooleanTerrainEditor
    {
        /// <summary>
        /// 当前的形状控制器
        /// </summary>
        private IShapeController _currentController
        {
            get {
                if (_currentShapeTool == null) return NoneShapeController.Instance;
                return _currentShapeTool.controller;
            }
        }
        private ShapeTool _currentShapeTool;
        private MeshFilter _meshFilter;
        private BooleanTerrain _booleanTerrain;
        
        /// <summary>
        /// 更换形状工具
        /// 触发时机：
        /// 1.在编辑网格时，点击shapeTool，更换后正在编辑的网格将变换为新的shapeTool适配的网格
        /// 2.选择已有形状时，将重新启用该形状的编辑功能
        /// </summary>
        /// <param name="shapeTool"></param>
        private void ChagneShapeTool(ShapeTool shapeTool)
        {
            if (this._currentShapeTool != shapeTool) {
                this._currentShapeTool = shapeTool;
                RefreshMesh();
            }
        }

        /// <summary>
        /// 刷新Mesh
        /// </summary>
        private void RefreshMesh()
        {
            //EditorUtility.SetDirty(_meshFilter);
        }
        

        private void SelectDefaultShapeTool()
        {
            OnShapeToolChanged(_firstShapeToolButton, BooleanTerrainEditorSO.Instance.shapeTools[0]);
        }
    }
}