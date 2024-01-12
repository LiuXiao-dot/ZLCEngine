using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
namespace ZLCEditor.Inspector.VisualElements
{
    internal static class VisualElementUtils
    {
        private static readonly HashSet<string> s_usedNames = new HashSet<string>();
        private static readonly Type s_FoldoutType = typeof(Foldout);
        //private static readonly string s_InspectorElementUssClassName = "unity-inspector-element";


        internal static int GetFoldoutDepth(this VisualElement element)
        {
            int foldoutDepth = 0;
            if (element.parent != null) {
                for (VisualElement parent = element.parent; parent != null; parent = parent.parent) {
                    if (s_FoldoutType.IsAssignableFrom(parent.GetType()))
                        ++foldoutDepth;
                }
            }
            return foldoutDepth;
        }
    }
}