#if UNITY_EDITOR
using System;
using UnityEngine;
namespace ZLCEngine.UGUISystem
{
    /// <summary>
    ///     按钮
    /// </summary>
    [Serializable]
    public class ZButton : AZUI
    {

        /// <summary>
        ///     文本的样式
        /// </summary>
        public ZLabel zLabel;

        /// <summary>
        ///     icon
        /// </summary>
        public Sprite icon;

        /// <summary>
        ///     background
        /// </summary>
        public Sprite bg;

        public override void RefreshUI()
        {

        }
        [Flags]
        internal enum Styles
        {
            BG = 1, // 是否有背景图片
            ICON = 2, // 是否有按钮图片
            TXT = 4 // 是否有文本
        }

        /// <summary>
        ///     同时有TXT和ICON时的对齐方式
        /// </summary>
        [Flags]
        internal enum Alignment
        {
            UP, // TXT在ICON上方
            DOWN,
            LEFT,
            RIGHT,
            OVERLAP // 重叠
        }
    }
}
#endif