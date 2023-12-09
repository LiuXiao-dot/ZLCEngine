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
namespace ZLCEditor.WindowSystem.ZLCEditor.WindowSystem
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

        [Button("刷新代码")]
        private void RefreshCode()
        {
            RefreshCode(false);
        }

        private void RefreshCode(bool isAuto)
        {
            if (layers == null) return;
            foreach (var layer in layers) {
                var gos = layer.gos;
                if (gos == null) continue;
                foreach (var windowGo in gos) {
                    windowGo.RefreshCode();
                }
            }
            if (!isAuto)
                EditorUtility.DisplayDialog("窗口", "代码生成完毕", "确定");
        }


        [Button("检测Prefab上是否都挂了View脚本")]
        private void CheckViews()
        {
            CheckViews(false);
        }

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
        [Button("初始化")]
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
        }

        [Button("清除")]
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
    }
}