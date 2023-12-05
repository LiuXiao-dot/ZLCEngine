using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using ZLCEngine.ConfigSystem;
using ZLCEngine.WindowSystem;
namespace ZLCEditor.WindowSystem
{
    /// <summary>
    /// 窗口按层级分组的工具
    /// </summary>
    [CreateAssetMenu]
    [Tool("窗口")]
    public class WindowLayerTool : ScriptableObject
    {
        [ReadOnly]
        public WindowLayer layer;

        public List<WindowGo> gos;

        [Button("创建窗口")]
        private void CreateWindow()
        {
            InputDialog.ShowWindow(value =>
            {
                // 生成view和ctl代码，再将view附加到GameObject上
                gos ??= new List<WindowGo>();
                gos.Add(WindowGo.Create(value, layer, gos.Count));
            });
        }
    }
}