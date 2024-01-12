using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
namespace ZLCEditor.Inspector.VisualElements
{
    /// <summary>
    /// ZLCPopupWindow的内容
    /// </summary>
    public abstract class ZLCPopupWindowContent
    {
        public EditorWindow editorWindow { get; internal set; }

        public abstract VisualElement CreateUI();
        public virtual Vector2 GetWindowSize()
        {
            return new Vector2(400, 200);
        }

        public virtual void OnOpen() {}
        public virtual void OnClose() {}
    }
    
    /// <summary>
    /// 基于UIToolkit的弹出窗口
    /// </summary>
    public class ZLCPopupWindow : EditorWindow
    {
        ZLCPopupWindowContent _windowContent;
        static double _lastClosedTime;
        static Rect _lastActivatorRect;
        static Rect _activatorRect;

        internal ZLCPopupWindow()
        {
        }

        public static ZLCPopupWindow Show(Rect activatorRect, ZLCPopupWindowContent windowContent)
        {
            return Show(activatorRect, windowContent, null);
        }

        internal static ZLCPopupWindow Show(Rect activatorRect, ZLCPopupWindowContent windowContent, PopupLocation[] locationPriorityOrder)
        {
            return Show(activatorRect, windowContent, locationPriorityOrder, ShowMode.PopupMenu);
        }

        // Shown on top of any previous windows
        internal static ZLCPopupWindow Show(Rect activatorRect, ZLCPopupWindowContent windowContent, PopupLocation[] locationPriorityOrder, ShowMode showMode)
        {
            // If we already have a popup window showing this type of content, then just close
            // the existing one.
            var existingWindows = Resources.FindObjectsOfTypeAll(typeof(ZLCPopupWindow));
            if (existingWindows != null && existingWindows.Length > 0) {
                var existingPopup = existingWindows[0] as ZLCPopupWindow;
                if (existingPopup != null && existingPopup._windowContent != null && windowContent != null) {
                    if (existingPopup._windowContent.GetType() == windowContent.GetType()) {
                        existingPopup.CloseWindow();
                        return null;
                    }
                }
            }

            if (ShouldShowWindow(activatorRect)) {
                ZLCPopupWindow win = CreateInstance<ZLCPopupWindow>();
                if (win != null) {
                    win.Init(activatorRect, windowContent, locationPriorityOrder);
                }
                if (Event.current != null) {
                    EditorGUIUtility.ExitGUI(); // Needed to prevent GUILayout errors on OSX
                }
                return win;
            }
            return null;
        }

        internal static bool ShouldShowWindow(Rect activatorRect)
        {
            const double kJustClickedTime = 0.2;
            bool justClosed = (EditorApplication.timeSinceStartup - _lastClosedTime) < kJustClickedTime;
            if (!justClosed || activatorRect != _lastActivatorRect) {
                _lastActivatorRect = activatorRect;
                return true;
            }
            return false;
        }

        internal void Init(Rect activatorRect, ZLCPopupWindowContent windowContent, PopupLocation[] locationPriorityOrder)
        {
            hideFlags = HideFlags.DontSave;
            wantsMouseMove = true;
            _windowContent = windowContent;
            _windowContent.editorWindow = this;
            _windowContent.OnOpen();
            _activatorRect = GUIUtility.GUIToScreenRect(activatorRect);
            
            rootVisualElement.Add(_windowContent.CreateUI());
            ShowAsDropDown(_activatorRect, _windowContent.GetWindowSize(), locationPriorityOrder);
        }
        
        protected void CloseWindow()
        {
            Close();
        }

        protected virtual void OnEnable()
        {
            AssemblyReloadEvents.beforeAssemblyReload += CloseWindow;
        }

        protected virtual void OnDisable()
        {
            AssemblyReloadEvents.beforeAssemblyReload -= CloseWindow;

            _lastClosedTime = EditorApplication.timeSinceStartup;
            CloseContent();
        }

        // Change to private protected once available in C#.
        internal void CloseContent()
        {
            if (_windowContent != null)
                _windowContent.OnClose();
        }
    }
}