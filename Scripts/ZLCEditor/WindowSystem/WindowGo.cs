using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using ZLCEditor.FormatSystem;
using ZLCEngine.Inspector;
using ZLCEngine.Utils;
using ZLCEngine.WindowSystem;
namespace ZLCEditor.WindowSystem
{
    /// <summary>
    /// 窗口GameObject的相关数据
    /// </summary>
    [System.Serializable]
    public class WindowGo
    {
        /// <summary>
        /// 用于判断GameObject是否发生了改变
        /// </summary>
        [HideInInspector]
        public long modifiedTime;

        /// <summary>
        /// 窗口的Prefab
        /// </summary>
        [ReadOnly]
        public GameObject prefab;

        /// <summary>
        /// Crl类型的代码
        /// </summary>
        [ReadOnly]
        public MonoScript ctlCode;

        /// <summary>
        /// View类型的代码
        /// </summary>
        [ReadOnly]
        public MonoScript viewCode;

        /// <summary>
        /// 窗口层级
        /// </summary>
        [ReadOnly]
        public WindowLayer layer;

        /// <summary>
        /// 窗口ID
        /// </summary>
        [ReadOnly]
        public int id;

        private WindowGo()
        {
            // 在将View代码添加到Prefab上之后第一次赋值
            modifiedTime = -1;
        }

        public static WindowGo Create(string name, WindowLayer layer, int baseId)
        {
            var go = new GameObject(name);
            go.AddComponent<RectTransform>();
            var canvas = go.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            WindowGoConverterManager.GenerateCode(go, out var ctlCode, out var viewCode);

            var windowGo = new WindowGo
            {
                prefab = PrefabUtility.SaveAsPrefabAsset(go, Path.Combine(Constant.PrefabURL, layer.ToString(), $"{go.name}.prefab")),
                ctlCode = ctlCode,
                viewCode = viewCode,
                layer = layer,
                id = baseId + (int)layer * 100000
            };

            CompilationPipeline.RequestScriptCompilation(); // 编译view,ctl代码
            GameObject.DestroyImmediate(go);
            return windowGo;
        }

        /// <summary>
        /// 删除窗口
        /// </summary>
        [Button("删除窗口")]
        internal void DeleteWindow()
        {
            // 删除prefab，ctlCode,viewCode 并从列表中移除，并刷新窗口枚举类型
            if (prefab != null) AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(prefab));
            if (ctlCode != null) AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(ctlCode));
            if (viewCode != null) AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(viewCode));
            // 从WindowLayerTool中删除自己
            var windowLayer = WindowTool.Instance.layers.Where(t => t.layer == layer).ToArray()[0];
            windowLayer.gos.Remove(this);
        }

        /// <summary>
        /// 将View组件添加到Prefab上
        /// </summary>
        public void AddView2Prefab()
        {
            if (prefab.GetComponent<AWindowView>() != null) return;
            var viewClass = viewCode.GetClass();
            var view = (AWindowView)prefab.AddComponent(viewClass);
            view.windowLayer = layer;
            view.ID = id;

            // 应用RectTransform
            var preset = WindowTool.Instance.GetRectTransformPreset(layer);
            preset.ApplyTo(prefab.GetComponent<RectTransform>());
            PrefabUtility.SavePrefabAsset(prefab);
            modifiedTime = File.GetLastWriteTime(AssetDatabase.GetAssetPath(prefab)).Ticks;
        }

        /// <summary>
        /// 刷新代码
        /// </summary>
        public bool RefreshCode()
        {
            if (prefab == null) return false;
            var tempTime = File.GetLastWriteTime(AssetDatabase.GetAssetPath(prefab)).Ticks;
            if (tempTime == modifiedTime) return false;
            modifiedTime = tempTime;
            var viewCode = FormatManager.Convert<GameObject, WindowViewCode>(prefab);
            var viewPath = Path.Combine(global::ZLCEditor.Constant.ZLCGenerateURL, Constant.ViewCodeURL, $"{prefab.name}View.cs");
            var absoluteViewPath = Path.Combine(global::ZLCEditor.Constant.BasePath, viewPath);
            FileHelper.SaveFile(viewCode.code, absoluteViewPath);
            AssetDatabase.Refresh();
            CompilationPipeline.RequestScriptCompilation(); // 编译view,ctl代码

            Debug.Log($"{prefab.name}生成代码完成");
            return true;
        }

        /// <summary>
        /// 同步组件到View的字段中
        /// </summary>
        public bool SyncComponent()
        {
            if (EditorApplication.isCompiling) {
                EditorUtility.DisplayDialog("窗口", "编译中，请稍等", "确定");
                return false;
            }
            ;
            var view = prefab.GetComponent<AWindowView>();
            if (view == null || RefreshCode()) {
                AddView2Prefab();
                EditorUtility.DisplayDialog("窗口", "由于prefab上没有添加AWindowView脚本，进行了代码同步，请等待编译完成后再次点击", "确定");
                return false;
            }
            var components = prefab.GetComponentsInChildren<WindowComponent>();
            foreach (var component in components) {
                var temps = component.components;
                var goName = component.gameObject.name;
                foreach (var temp in temps) {
                    var fieldName = $"{goName}_{temp.GetType().Name}";
                    view.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public).SetValue(view, temp);
                }
            }
            PrefabUtility.SavePrefabAsset(prefab);
            modifiedTime = File.GetLastWriteTime(AssetDatabase.GetAssetPath(prefab)).Ticks;
            return true;
        }
    }
}