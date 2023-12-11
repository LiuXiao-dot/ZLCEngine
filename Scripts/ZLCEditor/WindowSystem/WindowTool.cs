using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Presets;
using UnityEditorInternal;
using UnityEngine;
using ZLCEngine.ConfigSystem;
using ZLCEngine.Inspector;
using ZLCEngine.Utils;
using ZLCEngine.WindowSystem;
using FilePathAttribute = ZLCEngine.ConfigSystem.FilePathAttribute;
namespace ZLCEditor.WindowSystem
{
    /// <summary>
    /// 窗口工具
    /// </summary>
    [Tool("窗口", true)]
    [FilePath(FilePathAttribute.PathType.XWEditor, true)]
    public class WindowTool : SOSingleton<WindowTool>
    {
        [ReadOnly] public AssemblyDefinitionAsset assembly;
        [ReadOnly]
        [SerializeField] internal WindowLayerTool[] layers;

        [ReadOnly] public Preset[] rectTransformPresets;  

        private void Reset()
        {
            ZLCGenerateManager.CheckAsmdef();
            assembly = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(ZLCGenerateManager.path);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            layers = AssetDatabase.LoadAllAssetRepresentationsAtPath(FilePathAttribute.GetPath(typeof(WindowTool))).Select(t => (WindowLayerTool)t).ToArray();
           
            CheckViews(true);
            RefreshCode(true);
        }

        /*[Button("刷新代码")]
        private void RefreshCode()
        {
            RefreshCode(false);
        }*/

        /// <summary>
        /// 刷新代码
        /// </summary>
        /// <param name="isAuto"></param>
        /// <returns>false:未更新代码 true:已更新代码</returns>
        private bool RefreshCode(bool isAuto)
        {
            if (layers == null) return false;
            var refreshed = false;
            foreach (var layer in layers) {
                var gos = layer.gos;
                if (gos == null) continue;
                foreach (var windowGo in gos) {
                    refreshed |= windowGo.RefreshCode();
                }
            }
            if (!isAuto && refreshed)
                EditorUtility.DisplayDialog("窗口同步", "代码生成完毕,请等待编译完成再次一键刷新", "确定");
            return refreshed;
        }


        /*[Button("检测Prefab上是否都挂了View脚本")]
        private void CheckViews()
        {
            CheckViews(false);
        }*/

        private void CheckViews(bool isAuto)
        {
            if (layers == null) return;
            foreach (var layer in layers) {
                var gos = layer.gos;
                if (gos == null) continue;
                foreach (var windowGo in gos) {
                    windowGo.AddView2Prefab();
                }
            }
            if (!isAuto)
                EditorUtility.DisplayDialog("窗口", "检测完毕", "确定");
        }

        /// <summary>
        /// 初始化窗口层级目录等
        /// </summary>
        [Button("初始化窗口工具")]
        private void Init()
        {
            // 创建目录
            //- 代码目录
            FileHelper.CheckDirectory(Constant.FullCtlCodeURL);
            FileHelper.CheckDirectory(Constant.FullViewCodeURL);
            
            //- prefab目录
            FileHelper.CheckDirectory(Constant.PrefabURL);
            var windowLayers = Enum.GetNames(typeof(WindowLayer));
            foreach (var windowLayer in windowLayers) {
                FileHelper.CheckDirectory(Path.Combine(Constant.PrefabURL,windowLayer));
            }

            Clear();
            // 创建各个层级对应的窗口WindowLayerTool
            layers = AssetDatabase.LoadAllAssetRepresentationsAtPath(FilePathAttribute.GetPath(typeof(WindowTool))).Select(t => (WindowLayerTool)t).ToArray();
            foreach (var windowLayer in windowLayers) {
                var tool = ScriptableObject.CreateInstance<WindowLayerTool>();
                tool.layer = Enum.Parse<WindowLayer>(windowLayer);
                tool.name = windowLayer;
                if (layers.Any(temp => temp.name == windowLayer)) {
                    continue;
                }
                tool.hideFlags = HideFlags.None;
                AssetDatabase.AddObjectToAsset(tool,this);
            }
            AssetDatabase.SaveAssets();
            layers = AssetDatabase.LoadAllAssetRepresentationsAtPath(FilePathAttribute.GetPath(typeof(WindowTool))).Select(t => (WindowLayerTool)t).ToArray();
            
            
            // 加载presets
            var folder = Constant.PresetURL;
            var assets = AssetDatabase.FindAssets("t:Preset", new string[]{folder});
            var length = assets.Length;
            rectTransformPresets = new Preset[length];
            for (int i = 0; i < length; i++) {
                var preset = AssetDatabase.LoadAssetAtPath<Preset>(AssetDatabase.GUIDToAssetPath(assets[i]));
                if(preset == null) continue;
                rectTransformPresets[i] = preset;
            }
        }

        private void Clear()
        {
            layers = AssetDatabase.LoadAllAssetRepresentationsAtPath(FilePathAttribute.GetPath(typeof(WindowTool))).Select(t => (WindowLayerTool)t).ToArray();
            foreach (var layer in layers) {
                AssetDatabase.RemoveObjectFromAsset(layer);
            }
            AssetDatabase.SaveAssets();
            layers = null;
        }

        public Preset GetRectTransformPreset(WindowLayer layer)
        {
            foreach (var preset in rectTransformPresets) {
                if (preset.name == layer.ToString()) {
                    return preset;
                }
            }
            return default;
        }

        //[Button("同步组件到View中")]
        private void SyncComponents()
        {
            if (layers == null) return;
            var synced = false;
            foreach (var layer in layers) {
                var gos = layer.gos;
                if (gos == null) continue;
                foreach (var windowGo in gos) {
                    synced |= windowGo.SyncComponent();
                }
            }
            if(synced)
                EditorUtility.DisplayDialog("窗口同步", "同步组件到View中结束", "确定");
        }

        private void SyncWindows()
        {
            var folder = Constant.PrefabURL;
            var layerNames = Enum.GetNames(typeof(WindowLayer));
            var length = layerNames.Length;
            for (int i = 0; i < length; i++) {
                var index = i;
                var layer = layerNames[index];
                var promot = $"dir={folder}/{layer} ext:prefab";
                var layerList = this.layers.ToList();
                EditorHelper.SearchPackagesAssets<GameObject>(promot, temp =>
                {
                    var gos = layerList.Find(t=>t.layer == (WindowLayer)index).gos;
                    if(gos == null) return;
                    
                    // 有prefab，没有数据的，添加到gos中
                    var unbinds = temp.Where(t => gos.FindIndex(t2 => t2.prefab == t) == -1).ToList();
                    foreach (var unbind in unbinds) {
                        var bindGo = WindowGo.Create(unbind.name, (WindowLayer)index, gos.Count);
                        bindGo.prefab = unbind;
                        gos.Add(bindGo);
                    }
                    if (unbinds.Any()) {
                        EditorUtility.DisplayDialog("窗口同步", "绑定窗口完成", "确定");
                    }
                    
                    // 仅有数据，没有prefab的，直接删除
                    var allDelete = false;
                    var olds = gos.FindAll(t => !temp.Contains(t.prefab));
                    if(olds.Count == 0) return;
                    allDelete = EditorUtility.DisplayDialog("窗口同步", "是否删除全部旧窗口及代码？", "确定","取消");
                    foreach (var old in olds) {
                        // 检测是否存在代码，存在代码也删除，但会先弹出提示
                        if (allDelete) {
                            if (EditorUtility.DisplayDialog("窗口同步", $"是否删除{old.id}窗口及代码？", "确定", "跳过")) {
                                old.DeleteWindow();
                            }
                        }
                    }
                });
            }
        }

        /// <summary>
        /// 一键更新
        /// </summary>
        [Button("一键刷新")]
        private void Update()
        {
            if (EditorApplication.isCompiling) {
                EditorUtility.DisplayDialog("窗口同步", "请等待编译结束", "确认");
                return;
            }
            // 检测数据与实际的prefab是否相匹配，将不在数据中的prefab添加到数据中
            SyncWindows();
            
            // 检测数据中的窗口是否都有对应的代码，如果没有则生成，有则检测是否要更新代码
            if (RefreshCode(false)) {
                return;
            }
            
            // 检测prefab上是否有挂载View组件，如果没有则挂载
            CheckViews(false);

            // 检测view的字段是否与Prefab上的组件同步了，没有则同步
            SyncComponents();
        }
    }
}