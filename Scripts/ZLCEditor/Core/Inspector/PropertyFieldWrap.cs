using System;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
namespace ZLCEditor.Inspector
{
    public class PropertyFieldWrap
    {
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