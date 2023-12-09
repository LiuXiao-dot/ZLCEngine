using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using ZLCEditor.Inspector.VisualElements;
using Object = UnityEngine.Object;
namespace ZLCEditor.Inspector.Menu
{
    public class TwoPanelWindow : EditorWindow
    {

        private void CreateGUI()
        {
            // 添加样式表
            var splitterViewStyleSheet = EditorGUIUtility.Load(Path.Combine(Constant.USSPath, "SplitterView.uss")) as StyleSheet;
            rootVisualElement.styleSheets.Add(splitterViewStyleSheet);

            // 创建窗口
            var splitView = new SplitterView();
            var leftTreeView = new TreeView();
            var rightView = new ScrollView();
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

        private VisualElement currentContent;

        private void OnSelectionChanged(IEnumerable<object> objs)
        {
            var rightView = rootVisualElement.Q<VisualElement>("right-view");
            if (currentContent != null) {
                rightView.Remove(currentContent);
                currentContent = null;
            }
            var obj = objs.First();
            //if (obj == null || ((ZLCMenuItem)obj).target is not Object) return;
            if (obj == null) return;
            var target = ((ZLCMenuItem)obj).target;
            if (target is Object targetObj) {
                currentContent = new InspectorElement(targetObj);
            } else {
                var instance = ScriptableObject.CreateInstance<ZLCObject>();
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