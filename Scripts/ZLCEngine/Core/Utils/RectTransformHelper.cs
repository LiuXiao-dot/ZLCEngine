using UnityEngine;
namespace ZLCEngine.Utils
{
    /// <summary>
    ///     RectTransform的工具方法
    /// </summary>
    public sealed class RectTransformHelper
    {
        /// <summary>
        ///     设置<paramref name="rectTransform" />为向上对齐
        /// </summary>
        /// <param name="rectTransform"></param>
        public static void SetTopStretch(RectTransform rectTransform)
        {
            rectTransform.pivot = Vector2.up;
            rectTransform.anchorMin = Vector2.up;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }
        
        public static void SetStretchStretch(RectTransform rectTransform)
        {
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }
    }
}