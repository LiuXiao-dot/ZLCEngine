/*using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using ZLCEditor.Inspector;
using ZLCEditor.Inspector.VisualElements;
using ZLCEngine.TerrainSystem.Boolean;
namespace ZLCEditor.TerrainSystem.Boolean
{
    /// <summary>
    /// 布尔运算的地形编辑器
    /// </summary>
    [CustomEditor(typeof(BooleanTerrain))]
    public class BooleanTerrainEditor : BaseZLCEditor
    {
        private const int BORDER = 64;
        /// <summary>
        /// 当前的地形编辑器
        /// </summary>
        internal static BooleanTerrainEditor _terrainEditor;
        private ShapeTool _curShapeTool;
        
        protected override void CreateInspectorGUI(VisualElement root)
        {
            var shapes = serializedObject.FindProperty("shapes");
            
            var config = BooleanTerrainEditorSO.Instance;
            var shapeTools = config.shapeTools;
            if (shapeTools != null) {
                var toolGroup = new GroupBox("工具");
                toolGroup.AddToClassList("zlc-group-box");
                var shapeToolsVE = new ListView(shapeTools);
                shapeToolsVE.makeItem = ()=>new Button();
                shapeToolsVE.fixedItemHeight = BORDER;
                shapeToolsVE.bindItem = (item, index) =>
                {
                    CreateShapeToolBtn((Button)item, shapeTools[index]);
                };
                shapeToolsVE.selectionChanged += OnShapeToolChanged;
                toolGroup.Add(shapeToolsVE);
                root.Add(toolGroup);
            }

            var btnGroup = new GroupBox("按钮");
            btnGroup.AddToClassList("zlc-group-box");
            root.Add(btnGroup);
            btnGroup.Add(CreateOpenTerrainWindowBtn());
            btnGroup.Add(CreateSaveShapeToolControllerBtn());
            btnGroup.Add(CreateGenerateBtn());

            {
                var terrainGroup = new GroupBox("地形");
                terrainGroup.AddToClassList("zlc-group-box");
                terrainGroup.Add(new ZLCPropertyField(shapes));
                root.Add(terrainGroup);
            }
        }

        /// <summary>
        /// 打开地形编辑窗口
        /// </summary>
        /// <returns></returns>
        private VisualElement CreateOpenTerrainWindowBtn()
        {
            var btn = new Button();
            btn.text = "编辑地形";
            btn.RegisterCallback<ClickEvent>(_ =>
            {
                // 打开编辑地形窗口
                TerrainView.Open();
            });
            return btn;
        }
        
        /// <summary>
        /// 保存当前的形状控制器
        /// </summary>
        /// <returns></returns>
        private VisualElement CreateSaveShapeToolControllerBtn()
        {
            var btn = new Button();
            btn.text = "保存当前网格";
            btn.RegisterCallback<ClickEvent>(_ =>
            {
                // 保存当前正在编辑的网格
                
            });
            return btn;
        }

        /// <summary>
        /// 生成Mesh按钮
        /// </summary>
        private VisualElement CreateGenerateBtn()
        {
            var btn = new Button();
            btn.text = "生成网格";
            btn.RegisterCallback<ClickEvent>(_ =>
            {
                // 将已有Mesh合并
                
            });
            return btn;
        }
        
        private void OnShapeToolChanged(IEnumerable<object> obj)
        {
            _curShapeTool = obj.First() as ShapeTool;
        }

        private void CreateShapeToolBtn(Button btn, ShapeTool shapeTool)
        {
            btn.style.width = BORDER;
            btn.style.height = BORDER;
            if (shapeTool.icon != null)
                btn.style.backgroundImage = new StyleBackground(shapeTool.icon);
            else {
                btn.style.backgroundImage = null;
                btn.text = ((System.Type)shapeTool.shapeType).Name;
            }
        }
    }
}*/