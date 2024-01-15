using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using ZLCEditor.Inspector;
using ZLCEditor.Inspector.VisualElements;
using ZLCEngine.TerrainSystem.Boolean;
namespace ZLCEditor.TerrainSystem.Boolean
{
    /// <summary>
    /// 地形编辑器
    /// 数据存储为Shape,加载与编辑时需要创建Mesh
    ///
    /// partial:这部分放UI代码
    /// </summary>
    [CustomEditor(typeof(BooleanTerrain))]
    public partial class BooleanTerrainEditor : BaseZLCEditor
    {
        private const int BORDER = 64;
        private const string EditorName = "boolean-terrain-editor";
        private const string EditingAreaName = EditorName + "_editing";
        /// <summary>
        /// 当前激活的地形编辑器
        /// </summary>
        internal static BooleanTerrainEditor _currentTerrainEditor;

        /// <summary>
        /// 第一个形状工具按钮
        /// </summary>
        private Button _firstShapeToolButton;
        private Button _currentShapeBtn;
        private SerializedObject _currrentEditing;

        private delegate void ChangeEditingShape(IShape shape);
        private ChangeEditingShape _changeEditingShape;
        
        protected override VisualElement CreateGUI()
        {
            _booleanTerrain = (BooleanTerrain)serializedObject.targetObject;
            _meshFilter = _booleanTerrain.GetComponent<MeshFilter>();
            _currentTerrainEditor = this;
            //ResetController();
            var root = new VisualElement();
            root.name = EditorName;
            
            root.Add(DrawEditingShape());
            root.Add(DrawShapeTools());
            root.Add(DrawTools());
            root.Add(DrawShapes());
            return root;
        }

        /// <summary>
        /// 绘制当前正在编辑的形状的信息
        /// </summary>
        /// <returns></returns>
        private VisualElement DrawEditingShape()
        {
            var group = new GroupBox("编辑中...");
            group.AddToClassList("zlc-group-box");
            var empty = new VisualElement();
            empty.name = EditingAreaName;
            group.Add(empty);
            group.Add(CreateBtn(("重新生成", _ =>
            {
                _currrentEditing.ApplyModifiedProperties();
                _currentController.RefreshMesh();
                _meshFilter.mesh = _currentController.GetMesh();
                _changeEditingShape?.Invoke(_currentController.GetShape());
            })));
            
            _changeEditingShape = shape =>
            {
                var ve = group.Q(EditingAreaName);
                group.Remove(ve);
                ve = ZLCDrawerHelper.CreateDrawer(shape, out _currrentEditing);
                ve.name = EditingAreaName;
                group.Insert(1,ve);
            };
            return group;
        }
        
        /// <summary>
        /// 绘制可选的形状控制器
        /// </summary>
        /// <returns></returns>
        private VisualElement DrawShapeTools()
        {    
            // 创建形状工具按钮
            VisualElement CreateShapeToolBtn(ShapeTool shapeTool)
            {
                Button btn = new Button();
                btn.style.width = BORDER;
                btn.style.height = BORDER;
                if (shapeTool.icon != null)
                    btn.style.backgroundImage = new StyleBackground(shapeTool.icon);
                else {
                    btn.style.backgroundImage = null;
                    btn.text = ((System.Type)shapeTool.shapeType).Name;
                }
                btn.RegisterCallback<ClickEvent>(e=>OnShapeToolChanged(btn, shapeTool));
                if (_firstShapeToolButton == null)
                    _firstShapeToolButton = btn;
                return btn;
            }
            
            var config = BooleanTerrainEditorSO.Instance;
            var shapeTools = config.shapeTools;
            if (shapeTools != null) {
                var toolGroup = new GroupBox("形状工具");
                toolGroup.AddToClassList("zlc-group-box");
                var horizontal = new VisualElement();
                horizontal.AddToClassList("zlc-horizontal");
                toolGroup.Add(horizontal);
                foreach (var shapeTool in shapeTools) {
                    horizontal.Add(CreateShapeToolBtn(shapeTool));
                }
                return toolGroup;
            }
            return null;
        }

                   
        // 生成Mesh按钮
        private VisualElement CreateBtn((string name, EventCallback<ClickEvent> eventCallback) btnData)
        {
            var btn = new Button();
            btn.text = btnData.name;
            btn.RegisterCallback<ClickEvent>(btnData.eventCallback);
            return btn;
        }

        void OnShapeToolChanged(Button btn, ShapeTool shapeTool)
        {
            if (_currentShapeBtn != null) {
                _currentShapeBtn.SetEnabled(true);
            }
            _currentShapeBtn = btn;
            _currentShapeBtn.SetEnabled(false);
            ChagneShapeTool(shapeTool);
        }
        
        /// <summary>
        /// 绘制其他工具
        /// </summary>
        /// <returns></returns>
        private VisualElement DrawTools()
        {
            (string name,EventCallback<ClickEvent> eventCallback)[] tools = new (string name,EventCallback<ClickEvent> eventCallback)[]
            {
                ("创建新形状", _ => {
                    if (_currentShapeTool == null) {
                        SelectDefaultShapeTool();
                    }
                    // 让形状控制器创建一个新的形状，丢弃原有形状的控制权
                    _currentController.CreateNewShape();
                    _changeEditingShape?.Invoke(_currentController.GetShape());
                } ),
                ("保存当前形状", _ =>{
                    
                }),
                ("打开编辑地形窗口",_ =>
                {
                    // 打开编辑地形窗口
                    TerrainView.Open();
                }),
                ("生成网格",_ =>
                {
                    // 将已有Mesh合并并保存成文件
                
                })
            };
 
            var btnGroup = new GroupBox("工具");
            btnGroup.AddToClassList("zlc-group-box");
            foreach (var tool in tools) {
                btnGroup.Add(CreateBtn(tool));
            }
            
            return btnGroup;
        }

        /// <summary>
        /// 绘制已有形状
        /// </summary>
        /// <returns></returns>
        private VisualElement DrawShapes()
        {
            var shapes = serializedObject.FindProperty("shapes");
            var terrainGroup = new GroupBox("地形");
            terrainGroup.AddToClassList("zlc-group-box");
            terrainGroup.Add(new ZLCPropertyField(shapes));
            return terrainGroup;
        }
    }
}