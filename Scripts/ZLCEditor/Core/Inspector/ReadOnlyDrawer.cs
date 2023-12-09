using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using ZLCEngine.Inspector;
namespace ZLCEditor.Inspector
{
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            root.SetEnabled(false);
            root.name = "readonly";
            root.Add(new PropertyField(property));
            return root;
        }
    }
}