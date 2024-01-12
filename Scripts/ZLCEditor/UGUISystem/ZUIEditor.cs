using System;
using UnityEditor;
using UnityEngine;
using ZLCEngine.UGUISystem;
namespace ZLCEditor.UGUISystem
{
    [CustomEditor(typeof(ZUI))]
    public class ZUIEditor : Editor
    {
        private Editor _currentZUIEditor;
        private SerializedProperty _uiCache;

        private GUIContent _uiStyleChooseBtn;
        private SerializedProperty _uiType;

        private void OnEnable()
        {
            _uiStyleChooseBtn = new GUIContent();
            _uiStyleChooseBtn.text = "UI样式选择器";
            _uiCache = serializedObject.FindProperty("uiCache");
            _uiType = serializedObject.FindProperty("uiType");
        }

        public override void OnInspectorGUI()
        {
            EditorStyles.miniButton.CalcMinMaxWidth(_uiStyleChooseBtn, out float minWidth, out float maxWidth);
            float minHeight = EditorStyles.miniButton.CalcHeight(_uiStyleChooseBtn, minWidth);
            bool changed = false;
            if (GUILayout.Button(_uiStyleChooseBtn, EditorStyles.miniButton)) {
                Type[] types =
                {
                    typeof(ZLabel), typeof(ZButton)
                };
                ChooseZUIWindow chooseZUIWindow = new ChooseZUIWindow(types, index =>
                {
                    Type selectType = types[index];
                    GameObject go = ((ZUI)serializedObject.targetObject).gameObject;
                    if (go.TryGetComponent(out AZUI old)) {
                        DestroyImmediate(old);
                    }
                    AZUI result = (AZUI)go.AddComponent(selectType);
                    ((ZUI)serializedObject.targetObject).ui = result;
                    changed = true;
                });
                PopupWindow.Show(new Rect(0, 0, minWidth, minHeight), chooseZUIWindow);
            }

            if (changed) {
                if (_currentZUIEditor != null) {
                    DestroyImmediate(_currentZUIEditor);
                }
                // 绘制AZUI
                _currentZUIEditor = CreateEditor(((ZUI)serializedObject.targetObject).ui);
            }
            serializedObject.ApplyModifiedProperties();
            if (_currentZUIEditor != null) {
                _currentZUIEditor.OnInspectorGUI();
            }

        }
        /// <summary>
        ///     选择ZUI样式的弹窗
        /// </summary>
        private class ChooseZUIWindow : PopupWindowContent
        {
            private Action<int> _onSelect;
            private Vector2 _scrollPos;
            private int _selectIndex;
            private string[] _uis;

            public ChooseZUIWindow(Type[] uis, Action<int> onSelect)
            {
                _uis = new string[uis.Length];
                for (int i = 0; i < uis.Length; i++) {
                    _uis[i] = uis[i].Name;
                }
                _onSelect = onSelect;
            }

            public override void OnGUI(Rect rect)
            {
                _scrollPos = GUILayout.BeginScrollView(_scrollPos);
                _selectIndex = GUILayout.SelectionGrid(_selectIndex, _uis, 2);
                GUILayout.EndScrollView();
                if (GUILayout.Button("确认")) {
                    _onSelect?.Invoke(_selectIndex);
                }
            }
        }
    }
}