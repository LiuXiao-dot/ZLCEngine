using System;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
namespace ZLCEditor.Inspector
{
    public class PropertyFieldWrap
    {
        /// <summary>
        ///     todo:添加更多支持的类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static VisualElement CreateFieldFromType(Type type)
        {
            if (type == typeof(int)) {
                return new IntegerField();
            }
            if (type == typeof(long)) {
                return new LongField();
            }
            if (type == typeof(bool)) {
                return new Toggle();
            }
            if (type == typeof(float)) {
                return new FloatField();
            }
            if (type == typeof(string)) {
                return new TextField();
            }
            if (type == typeof(Color)) {
                return new ColorField();
            }
            if (type == typeof(LayerMask)) {
                return new LayerMaskField();
            }
            if (type.IsEnum) {

            } else if (type == typeof(Vector2)) {

            } else if (type == typeof(Vector3)) {

            } else if (type == typeof(Vector4)) {

            } else if (type == typeof(Rect)) {

            } else if (type.IsArray) {

            } else if (type == typeof(Character)) {

            } else if (type == typeof(AnimationCurve)) {

            } else if (type == typeof(Bounds)) {

            } else if (type == typeof(Gradient)) {

            } else if (type == typeof(Quaternion)) {

            } else if (type == typeof(Vector2Int)) {

            } else if (type == typeof(Vector3Int)) {

            } else if (type == typeof(RectInt)) {

            } else if (type == typeof(BoundsInt)) {

            } else {
                return new ObjectField();
            }
            return null;
        }

        public static object GetFieldValue(VisualElement filed)
        {
            switch (filed) {
                case IntegerField intergerField:
                    return intergerField.value;
                case ObjectField objectField:
                    return objectField.value;
            }
            return null;
        }
    }
}