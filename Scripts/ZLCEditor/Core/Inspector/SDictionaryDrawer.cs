/*using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using ZLCEngine.SerializeTypes;
namespace ZLCEditor.Inspector
{
    [CustomPropertyDrawer(typeof(SDictionary<,>))]
    public class SDictionaryDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var listView = new ListView();
            var cache = property.serializedObject.FindProperty($"{property.propertyPath}._cache");
            return listView;
        }
    }
}*/