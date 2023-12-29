using UnityEngine.UIElements;
namespace ZLCEditor.Tool
{
    /// <summary>
    /// 工具的接口
    /// </summary>
    public interface ITool
    {
        /// <summary>
        /// 创建UI
        /// </summary>
        /// <returns></returns>
        VisualElement CreateGUI();
    }
}