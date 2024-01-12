using UnityEditor;
using UnityEngine.UIElements;
using ZLCEditor.Inspector.VisualElements;
using ZLCEngine.SerializeTypes;
namespace ZLCEditor.Inspector
{
    /// <summary>
    /// todo:添加数据时先打开一个窗口输入数据，判断合法性，再添加
    /// </summary>
    [CustomPropertyDrawer(typeof(SDictionary<,>.KV))]
    public class SDictionaryKVDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            root.AddToClassList("zlc-vertical");
            
            var keyView = new ZLCPropertyField(property.FindPropertyRelative("key"));
            keyView.AddToClassList("zlc-flex-height");
            keyView.name = "zlc-dic-key";
            var valueView = new ZLCPropertyField(property.FindPropertyRelative("value"));
            valueView.AddToClassList("zlc-flex-height");
            valueView.name = "zlc-dic-value";
            root.Add(keyView);
            root.Add(valueView);
            return root;
        }
    }
}