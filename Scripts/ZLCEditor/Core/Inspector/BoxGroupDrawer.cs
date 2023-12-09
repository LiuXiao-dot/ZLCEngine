using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using ZLCEngine.Inspector;
namespace ZLCEditor.Inspector
{
    /// <summary>
    /// UI元素组
    /// </summary>
    [CustomPropertyDrawer(typeof(BoxGroupAttribute))]
    public class BoxGroupDrawer : PropertyDrawer
    {
        private VisualElement _root;
        private string _groupName;
        private string _path;
        private bool inited;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            _root = new VisualElement();
            var boxGroupAttribute = (BoxGroupAttribute)attribute;
            _groupName = boxGroupAttribute.groupName;
            _path = property.propertyPath;
            _root.RegisterCallback<AttachToPanelEvent>(OnAttachedToPanel);
            _root.Add(new PropertyField(property));
            inited = false;
            return _root;
        }
        
        private void OnAttachedToPanel(AttachToPanelEvent evt)
        {
            if(inited)
                return;
            inited = true;
            VisualElement propertyField = _root.parent;
            while (propertyField != null && (propertyField is not PropertyField || propertyField.parent.parent is not InspectorElement)) {
                propertyField = propertyField.parent;
            }
            if(propertyField == null) return;
            VisualElement parent = propertyField.parent;
            if(parent == null) return;
            
            var groupBox = parent.Q<GroupBox>(_groupName);
            if (groupBox == null) {
                groupBox = new GroupBox();
                groupBox.name = _groupName;
                parent.Add(groupBox);
            }

            if(groupBox.Contains(propertyField)) return;
            var index = propertyField.parent.IndexOf(propertyField);
            groupBox.PlaceInFront(propertyField);
            groupBox.Add(propertyField);
            groupBox.text = _groupName;
            groupBox.Q<Label>().name = "title";
        }

    }
}