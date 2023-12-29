using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.Presets;
using UnityEngine;
using ZLCEditor.FormatSystem;
using ZLCEngine.Inspector;
using ZLCEngine.Utils;
using ZLCEngine.WindowSystem;
namespace ZLCEditor.WindowSystem
{
    /// <summary>
    ///     窗口GameObject的相关数据
    /// </summary>
    [Serializable]
    public class WindowGo
    {
        /// <summary>
        ///     用于判断GameObject是否发生了改变
        /// </summary>
        [HideInInspector]
        public long modifiedTime;
        
        /// <summary>
        /// 用于判断是否绑定了最新的组件
        /// </summary>
        [HideInInspector]
        public long combineTime;

        /// <summary>
        ///     窗口的Prefab
        /// </summary>
        [ReadOnly]
        public GameObject prefab;

        /// <summary>
        ///     Crl类型的代码
        /// </summary>
        [ReadOnly]
        public MonoScript ctlCode;

        /// <summary>
        ///     View类型的代码
        /// </summary>
        [ReadOnly]
        public MonoScript viewCode;

        /// <summary>
        ///     窗口层级
        /// </summary>
        [ReadOnly]
        public WindowLayer layer;

        /// <summary>
        ///     窗口ID
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
            GameObject go = new GameObject(name);
            go.layer = LayerMask.NameToLayer("UI");
            go.AddComponent<RectTransform>();
            Canvas canvas = go.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            WindowGoConverterManager.GenerateCode(go, out MonoScript ctlCode, out MonoScript viewCode);

            WindowGo windowGo = new WindowGo
            {
                prefab = PrefabUtility.SaveAsPrefabAsset(go, Path.Combine(Constant.PrefabURL, layer.ToString(), $"{go.name}.prefab")),
                ctlCode = ctlCode,
                viewCode = viewCode,
                layer = layer,
                id = baseId*ZLCEngine.WindowSystem.Constant.WindowNum + (int)layer * ZLCEngine.WindowSystem.Constant.LayerRatio
            };

            CompilationPipeline.RequestScriptCompilation(); // 编译view,ctl代码
            GameObject.DestroyImmediate(go);
            return windowGo;
        }

        /// <summary>
        ///     删除窗口
        /// </summary>
        [Button("删除窗口")]
        internal void DeleteWindow()
        {
            // 删除prefab，ctlCode,viewCode 并从列表中移除，并刷新窗口枚举类型
            if (prefab != null) AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(prefab));
            if (ctlCode != null) AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(ctlCode));
            if (viewCode != null) AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(viewCode));
            // 从WindowLayerTool中删除自己
            WindowLayerTool windowLayer = WindowTool.Instance.layers.Where(t => t.layer == layer).ToArray()[0];
            windowLayer.gos.Remove(this);
        }

        /// <summary>
        ///     将View组件添加到Prefab上
        /// </summary>
        public void AddView2Prefab()
        {
            if (prefab.GetComponent<AWindowView>() != null) return;
            Type viewClass = viewCode.GetClass();
            AWindowView view = (AWindowView)prefab.AddComponent(viewClass);
            view.windowLayer = layer;
            view.ID = id;

            // 应用RectTransform
            Preset preset = WindowTool.Instance.GetRectTransformPreset(layer);
            preset.ApplyTo(prefab.GetComponent<RectTransform>());
            PrefabUtility.SavePrefabAsset(prefab);
            modifiedTime = File.GetLastWriteTime(AssetDatabase.GetAssetPath(prefab)).Ticks;
        }

        /// <summary>
        ///     刷新代码
        /// </summary>
        public bool RefreshCode()
        {
            if (prefab == null) return false;
            long tempTime = File.GetLastWriteTime(AssetDatabase.GetAssetPath(prefab)).Ticks;
            if (tempTime == combineTime || modifiedTime == tempTime) return false; // 判断是否已绑定最新的数据，绑定了就返回
            modifiedTime = tempTime;
            WindowViewCode viewCode = FormatManager.Convert<GameObject, WindowViewCode>(prefab);
            string viewPath = Path.Combine(ZLCEditor.Constant.ZLCGenerateURL, Constant.ViewCodeURL, $"{prefab.name}View.cs");
            string absoluteViewPath = Path.Combine(ZLCEditor.Constant.BasePath, viewPath);
            FileHelper.SaveFile(viewCode.code, absoluteViewPath);
            AssetDatabase.Refresh();
            CompilationPipeline.RequestScriptCompilation(); // 编译view,ctl代码

            this.viewCode = AssetDatabase.LoadAssetAtPath<MonoScript>(viewPath);
            Debug.Log($"{prefab.name}生成代码完成");
            return true;
        }

        /// <summary>
        ///     同步组件到View的字段中
        /// </summary>
        public bool SyncComponent()
        {
            if (EditorApplication.isCompiling) {
                //EditorUtility.DisplayDialog("窗口", "编译中，请稍等", "确定");
                return false;
            }
            ;
            AWindowView view = prefab.GetComponent<AWindowView>();
            if (view == null || RefreshCode()) {
                AddView2Prefab();
                EditorUtility.DisplayDialog("窗口", "由于prefab上没有添加AWindowView脚本，进行了代码同步，请等待编译完成后再次点击", "确定");
                return false;
            }
            WindowComponent[] components = prefab.GetComponentsInChildren<WindowComponent>();
            foreach (WindowComponent component in components) {
                MonoBehaviour[] temps = component.components;
                string goName = component.gameObject.name;
                foreach (MonoBehaviour temp in temps) {
                    string fieldName = $"{goName}_{temp.GetType().Name}";
                    view.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public).SetValue(view, temp);
                }
            }
            PrefabUtility.SavePrefabAsset(prefab);
            combineTime = File.GetLastWriteTime(AssetDatabase.GetAssetPath(prefab)).Ticks;
            return true;
        }
    }
}