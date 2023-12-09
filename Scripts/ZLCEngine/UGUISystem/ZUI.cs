using System;
using UnityEditor;
using UnityEngine;
using ZLCEngine.SerializeTypes;
namespace ZLCEngine.UGUISystem
{
    /// <summary>
    /// ZLCEngine中的UI样式
    /// </summary>
    [Serializable]
    public class ZUI : MonoBehaviour, ISerializationCallbackReceiver
    {
        public AZUI ui;
        /// <summary>
        /// AZUI的序列化信息
        /// </summary>
        [SerializeField][HideInInspector]private string uiCache;
        [SerializeField][HideInInspector]private SType uiType;

        public void OnBeforeSerialize()
        {
            if (ui == null) {
                uiCache = null;
                return;
            }
            uiType = ui.GetType();
            // todo:使用自定义序列化工具
            uiCache = JsonUtility.ToJson(ui);
        }
        public void OnAfterDeserialize()
        {
            /*if (uiCache != null && uiCache.Length > 0) {
                ui = (AZUI)JsonUtility.FromJson(uiCache,uiType);
            }*/
        }
    }

    [CustomEditor(typeof(ZUI))]
    public class ZUIEditor : Editor
    {
        /// <summary>
        /// 选择ZUI样式的弹窗
        /// </summary>
        private class ChooseZUIWindow : PopupWindowContent
        {
            private int _selectIndex;
            private string[] _uis;
            private Vector2 _scrollPos;
            private Action<int> _onSelect;

            public ChooseZUIWindow(Type[] uis, Action<int> onSelect)
            {
                this._uis = new string[uis.Length];
                for (int i = 0; i < uis.Length; i++) {
                    this._uis[i] = uis[i].Name;
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

        private GUIContent _uiStyleChooseBtn;
        private SerializedProperty _uiCache;
        private SerializedProperty _uiType;
        private Editor _currentZUIEditor;

        private void OnEnable()
        {
            _uiStyleChooseBtn = new GUIContent();
            _uiStyleChooseBtn.text = "UI样式选择器";
            _uiCache = serializedObject.FindProperty("uiCache");
            _uiType = serializedObject.FindProperty("uiType");
        }

        public override void OnInspectorGUI()
        {
            EditorStyles.miniButton.CalcMinMaxWidth(_uiStyleChooseBtn,out var minWidth, out var maxWidth);
            var minHeight = EditorStyles.miniButton.CalcHeight(_uiStyleChooseBtn, minWidth);
            bool changed = false;
            if (GUILayout.Button(_uiStyleChooseBtn,EditorStyles.miniButton)) {
                var types = new Type[]
                {
                    typeof(ZLabel), typeof(ZButton),
                };
                var chooseZUIWindow = new ChooseZUIWindow(types, index =>
                {
                    var selectType = types[index];
                    var go = ((ZUI)serializedObject.targetObject).gameObject;
                    if (go.TryGetComponent<AZUI>(out var old)) {
                        DestroyImmediate(old);
                    }
                    var result = (AZUI)go.AddComponent(selectType);
                    ((ZUI)serializedObject.targetObject).ui = result;
                    changed = true;
                });
                PopupWindow.Show(new Rect(0,0,minWidth,minHeight),chooseZUIWindow);
            }

            if (changed) {
                if (_currentZUIEditor != null) {
                    DestroyImmediate(_currentZUIEditor);
                }
                // 绘制AZUI
                _currentZUIEditor = Editor.CreateEditor(((ZUI)serializedObject.targetObject).ui);
            }
            serializedObject.ApplyModifiedProperties();
            if (_currentZUIEditor != null) {
                _currentZUIEditor.OnInspectorGUI();
            }

        }
    }
}