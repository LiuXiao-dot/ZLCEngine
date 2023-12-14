using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using ZLCEditor.Inspector.VisualElements;
namespace ZLCEditor.Inspector.Menu
{
    public class TwoPanelWindow : EditorWindow
    {

        private VisualElement currentContent;

        private void CreateGUI()
        {
            // 添加样式表
            StyleSheet splitterViewStyleSheet = EditorGUIUtility.Load(Constant.ZLC_EDITOR_USS) as StyleSheet;
            rootVisualElement.styleSheets.Add(splitterViewStyleSheet);

            // 创建窗口
            SplitterView splitView = new SplitterView();
            TreeView leftTreeView = new TreeView();
            ScrollView rightView = new ScrollView();
            rightView.name = "right-view";
            splitView.leftPane.Add(leftTreeView);
            splitView.leftPane.style.width = 200;
            splitView.rightPane.Add(rightView);

            leftTreeView.AddToClassList(Constant.ZLC_TREE_VIEW);
            leftTreeView.SetRootItems(BuildMenuTree().GetItems());
            leftTreeView.makeItem = () => new Label();
            leftTreeView.bindItem = (element, i) =>
            {
                ((Label)element).text = leftTreeView.GetItemDataForIndex<ZLCMenuItem>(i).GetName();
            };
            leftTreeView.selectionChanged += OnSelectionChanged;

            rootVisualElement.Add(splitView);
        }

        private void OnSelectionChanged(IEnumerable<object> objs)
        {
            VisualElement rightView = rootVisualElement.Q<VisualElement>("right-view");
            if (currentContent != null) {
                rightView.Remove(currentContent);
                currentContent = null;
            }
            object obj = objs.First();
            //if (obj == null || ((ZLCMenuItem)obj).target is not Object) return;
            if (obj == null) return;
            object target = ((ZLCMenuItem)obj).target;
            if (target is Object targetObj) {
                currentContent = new InspectorElement(targetObj);
            } else {
                ZLCObject instance = CreateInstance<ZLCObject>();
                instance.t = target;
                currentContent = new InspectorElement(instance);
            }

            rightView.Add(currentContent);
        }

        protected virtual MenuTree BuildMenuTree()
        {
            return null;
        }
    }
}