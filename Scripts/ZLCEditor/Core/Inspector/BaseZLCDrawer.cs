using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using ZLCEngine.Inspector;
namespace ZLCEditor.Inspector
{
    [CustomPropertyDrawer(typeof(object), true)]
    public class BaseZLCDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            root.Add(new PropertyField(property));
            switch (property.propertyType) {
                case SerializedPropertyType.Generic:
                case SerializedPropertyType.Integer:
                case SerializedPropertyType.Enum:
                case SerializedPropertyType.Boolean:
                case SerializedPropertyType.Float:
                case SerializedPropertyType.String:
                case SerializedPropertyType.Color:
                case SerializedPropertyType.ObjectReference:
                case SerializedPropertyType.LayerMask:
                case SerializedPropertyType.Vector2:
                case SerializedPropertyType.Vector3:
                case SerializedPropertyType.Vector4:
                case SerializedPropertyType.Rect:
                case SerializedPropertyType.ArraySize:
                case SerializedPropertyType.Character:
                case SerializedPropertyType.AnimationCurve:
                case SerializedPropertyType.Bounds:
                case SerializedPropertyType.Gradient:
                case SerializedPropertyType.Quaternion:
                case SerializedPropertyType.ExposedReference:
                case SerializedPropertyType.FixedBufferSize:
                case SerializedPropertyType.Vector2Int:
                case SerializedPropertyType.Vector3Int:
                case SerializedPropertyType.RectInt:
                case SerializedPropertyType.BoundsInt:
                case SerializedPropertyType.ManagedReference:
                case SerializedPropertyType.Hash128:
                    break;
                default:
                    return root;
            }

            try {
                // -- 检测各个方法是否有被可序列化的特性，如果有则按对应的特性进行序列化，没有则跳过
                var methods = property.boxedValue.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(t => Attribute.IsDefined(t, typeof(AnySerializableAttribute), true));
                foreach (var method in methods) {
                    var drawerType = ScriptAttributeUtilityWrapper.GetDrawerTypeForType(method.GetCustomAttribute<AnySerializableAttribute>(true).GetType());
                    if (drawerType != null) {
                        var drawer = (IAnySerializableAttributeEditor)Activator.CreateInstance(drawerType);
                        var methodGUI = drawer.CreateGUI(method, property.boxedValue);
                        if (methodGUI != null)
                            root.Add(methodGUI);
                    }
                }
            }
            catch {
                // ignored
            }

            return root;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, true);
        }
    }
}