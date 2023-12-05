using System;
using UnityEditor;
using UnityEngine;
namespace ZLCEditor
{
    public class InputDialog : EditorWindow
    {
        class Styles
        {
            // public GUIStyle input = "Input";
            public GUIStyle label = "Label";
            //public GUIStyle button = "Large Button";
            public GUIContent create = EditorGUIUtility.TrTextContent("创建");

            public Styles()
            {
                /*input = new GUIStyle(GUI.skin.textField);
                input.fixedHeight = 64;*/
                label = new GUIStyle(GUI.skin.label);
            }
        }

        private static Styles _styles = null;
        private string value;
        private Action<string> onInput;
        private Action onCancel;

        public static void ShowWindow(Action<string> onInput, Action onCancel = null)
        {
            var window = EditorWindow.CreateWindow<InputDialog>();
            window.onInput = onInput;
            window.onCancel = onCancel;
            window.Show();
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

        private void OnDisable()
        {
            onCancel?.Invoke();
        }
    }
}