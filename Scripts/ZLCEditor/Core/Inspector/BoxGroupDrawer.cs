using UnityEditor;
using UnityEngine.UIElements;
using ZLCEngine.Inspector;
namespace ZLCEditor.Inspector
{
    [CustomPropertyDrawer(typeof(BoxGroupAttribute))]
    public class BoxGroupDrawer : DecoratorDrawer
    {
        private GroupBox _groupBox;

        public override VisualElement CreatePropertyGUI()
        {
            BoxGroupAttribute boxGroupAttribute = (BoxGroupAttribute)attribute;
            _groupBox = new GroupBox();
            _groupBox.name = boxGroupAttribute.groupName;
            _groupBox.text = boxGroupAttribute.groupName;
            _groupBox.AddToClassList("zlc-group-box");

            return _groupBox;
        }
    }
}