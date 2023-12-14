using System;
using UnityEditor;
using UnityEngine;
namespace ZLCEditor
{
    public class InputDialog : EditorWindow
    {

        private static Styles _styles;
        private Action onCancel;
        private Action<string> onInput;
        private string value;

        private void OnDisable()
        {
            onCancel?.Invoke();
        }

        private void OnGUI()
        {
            if (_styles == null) {
                _styles = new Styles();
            }

            value = EditorGUILayout.TextField(value);
            if (GUILayout.Button(_styles.create)) {
                onInput?.Invoke(value);
                Close();
            }
        }

        public static void ShowWindow(Action<string> onInput, Action onCancel = null)
        {
            InputDialog window = CreateWindow<InputDialog>();
            window.onInput = onInput;
            window.onCancel = onCancel;
            window.Show();
        }
        private class Styles
        {
            //public GUIStyle button = "Large Button";
            public GUIContent create = EditorGUIUtility.TrTextContent("创建");
            // public GUIStyle input = "Input";
            public GUIStyle label = "Label";

            public Styles()
            {
                /*input = new GUIStyle(GUI.skin.textField);
                input.fixedHeight = 64;*/
                label = new GUIStyle(GUI.skin.label);
            }
        }
    }
}