using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
namespace ZLCEditor.Inspector
{
    [CustomPropertyDrawer(typeof(object), true)]
    public class BaseZLCDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return ZLCDrawerHelper.CreateDrawer(property);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, true);
        }
    }
}