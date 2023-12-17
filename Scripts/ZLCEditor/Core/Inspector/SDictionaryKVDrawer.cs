using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using ZLCEngine.SerializeTypes;
namespace ZLCEditor.Inspector
{
    [CustomPropertyDrawer(typeof(SDictionary<,>.KV))]
    public class SDictionaryKVDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            root.AddToClassList("zlc-horizontal");
            
            var keyView = new PropertyField(property.FindPropertyRelative("key"));
            keyView.AddToClassList("zlc-flex");
            keyView.name = "zlc-dic-key";
            var valueView = new PropertyField(property.FindPropertyRelative("value"));
            valueView.AddToClassList("zlc-flex");
            valueView.name = "zlc-dic-value";
            root.Add(keyView);
            root.Add(valueView);
            return root;
        }
    }
}