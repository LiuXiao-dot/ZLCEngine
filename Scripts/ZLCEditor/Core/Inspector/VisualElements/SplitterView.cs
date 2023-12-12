using UnityEngine;
using UnityEngine.UIElements;
namespace ZLCEditor.Inspector.VisualElements
{
    /// <summary>
    /// 左右两栏的窗口
    /// </summary>
    public class SplitterView : VisualElement
    {
        public VisualElement leftPane { get; private set; }
        public VisualElement rightPane { get; private set; }

        private VisualElement _dragLine;

        private float _leftPaneWidth;

        public SplitterView()
        {
            name = "zlc-splitter";
            viewDataKey = "zlc-key-splitter";

            leftPane = new VisualElement();
            leftPane.name = "zlc-splitter-left-pane";
            Add(leftPane);

            var dragLineAnchor = new VisualElement();
            dragLineAnchor.name = "zlc-splitter-dragline-anchor";
            Add(dragLineAnchor);

            _dragLine = new VisualElement();
            _dragLine.name = "zlc-splitter-dragline";
            _dragLine.AddManipulator(new SquareResizer(this));
            dragLineAnchor.Add(_dragLine);

            rightPane = new VisualElement();
            rightPane.name = "zlc-splitter-right-pane";
            Add(rightPane);

            _leftPaneWidth = 400;
            leftPane.style.width = _leftPaneWidth;
        }

        class SquareResizer : MouseManipulator
        {
            private Vector2 _start;
            protected bool _active;
            private SplitterView _splitter;

            public SquareResizer(SplitterView splitter)
            {
                _splitter = splitter;
                activators.Add(new ManipulatorActivationFilter(){button = MouseButton.LeftMouse});
                _active = false;
            }

            protected override void RegisterCallbacksOnTarget()
            {
                target.RegisterCallback<MouseDownEvent>(OnMouseDown);
                target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
                target.RegisterCallback<MouseUpEvent>(OnMouseUp);
            }
            protected override void UnregisterCallbacksFromTarget()
            {
                target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
                target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
                target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
            }

            protected void OnMouseDown(MouseDownEvent e)
            {
                if (_active)
                {
                    e.StopImmediatePropagation();
                    return;
                }

                if (CanStartManipulation(e))
                {
                    _start = e.localMousePosition;

                    _active = true;
                    target.CaptureMouse();
                    e.StopPropagation();
                }
            }
            
            protected void OnMouseMove(MouseMoveEvent e)
            {
                if (!_active || !target.HasMouseCapture())
                    return;

                Vector2 diff = e.localMousePosition - _start;

                _splitter.leftPane.style.width = _splitter.leftPane.layout.width + diff.x;

                e.StopPropagation();
            }

            protected void OnMouseUp(MouseUpEvent e)
            {
                if (!_active || !target.HasMouseCapture() || !CanStopManipulation(e))
                    return;

                _active = false;
                target.ReleaseMouse();
                e.StopPropagation();
                
                _splitter._leftPaneWidth = _splitter.leftPane.resolvedStyle.width;
            }
        }
    }
}