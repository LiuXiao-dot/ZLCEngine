using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
namespace ZLCEditor.Inspector
{
    [CustomEditor(typeof(ZLCObject))]
    public class ZLCObjectEditor : Editor
    {
        private SerializedProperty _value;
        
        private void OnEnable()
        {
            _value =serializedObject.FindProperty("t");
        }

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            root.name = "zlc-object";
            root.Add(new PropertyField(_value));
            return root;
        }
    }
}