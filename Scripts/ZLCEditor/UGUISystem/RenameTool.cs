using System;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace ZLCEditor.UGUISystem
{
    public class RenameTool
    {
        private class RenameWindow : PopupWindowContent
        {
            private string _value = "";
            private Action<string> _callback;

            public override Vector2 GetWindowSize()
            {
                return new Vector2(100, 60);
            }

            public RenameWindow(Action<string> callback)
            {
                this._callback = callback;
            }

            public override void OnGUI(Rect rect)
            {
                _value = GUILayout.TextField(_value);
                if (GUILayout.Button("确认")) {
                    if (string.IsNullOrEmpty(_value)) {
                        return;
                    }
                    _callback?.Invoke(_value);
                }
            }
        }

        [MenuItem("Assets/重命名")]
        private static void Rename()
        {
            var selects = Selection.objects;

            var renameWindow = new RenameWindow(value =>
            {
                var index = 0;
                foreach (var select in selects) {
                    var path = AssetDatabase.GetAssetPath(select);
                    if (string.IsNullOrEmpty(path)) continue;
                    AssetDatabase.RenameAsset(path, $"{value}_{index}");
                    index++;
                }
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            });

            var projectBrowerType = typeof(EditorWindow).Assembly.GetType("UnityEditor.ProjectBrowser");
            var window = EditorWindow.GetWindow(projectBrowerType);
            PopupWindow.Show(new Rect(window.position.width / 2, 0, 100,60), renameWindow);
        }

    }
}