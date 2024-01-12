using ZLCEngine.ConfigSystem;
using ZLCEngine.TerrainSystem.Boolean;
namespace ZLCEditor.TerrainSystem.Boolean
{
    /// <summary>
    /// 布尔地形编辑器的配置
    /// </summary>
    [FilePath(FilePathAttribute.PathType.XWEditor,true)]
    public class BooleanTerrainEditorSO : SOSingleton<BooleanTerrainEditorSO>
    {
        public ShapeTool[] shapeTools;
    }

}