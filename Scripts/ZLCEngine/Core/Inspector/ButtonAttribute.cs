using System;
using UnityEngine;
namespace ZLCEngine.Inspector
{
    /// <summary>
    ///     编辑器按钮
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ButtonAttribute : AnySerializableAttribute
    {
        public int height;
        public Sprite icon;
        public string label;
        public int width;

        public ButtonAttribute() : this(100, 16, "")
        {

        }

        public ButtonAttribute(string label) : this(100, 16, label)
        {
        }

        public ButtonAttribute(Sprite icon) : this(100, 16, icon)
        {
        }

        public ButtonAttribute(int width, int height, string label)
        {
            this.width = width;
            this.height = height;
            this.label = label;
        }

        public ButtonAttribute(int width, int height, Sprite icon)
        {
            this.width = width;
            this.height = height;
            this.icon = icon;
        }
    }
}