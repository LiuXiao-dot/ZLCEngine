using UnityEditor;
using UnityEngine.UIElements;
using ZLCEditor.Inspector.VisualElements;
namespace ZLCEditor.Inspector
{
    /// <summary>
    /// 默认的object的绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(object), true)]
    public class ZLCDrawer
    {
        private SerializedProperty _serializedProperty;
        public ZLCDrawer(SerializedProperty serializedProperty)
        {
            this._serializedProperty = serializedProperty;
        }
        
        public virtual VisualElement CreateGUI()
        {
            return new ZLCPropertyField(this._serializedProperty);
        }
    }
}