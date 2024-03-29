using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.UIElements;
namespace ZLCEditor.Inspector.Menu
{
    /// <summary>
    ///     菜单树
    /// </summary>
    public class MenuTreeView : VisualElement
    {

        private TreeView _treeView;
        private DefaultTreeViewControllerWrap<VisualElement> _treeViewController;
        public bool hierarchyHasChanged { get; set; }

        public IList<TreeViewItemData<VisualElement>> treeRootItems { get; } = new List<TreeViewItemData<VisualElement>>();
        public IEnumerable<TreeViewItemData<VisualElement>> treeItems
        {
            get {
                foreach (int itemId in _treeView.viewController.GetAllItemIds()) {
                    yield return _treeViewController.GetTreeViewItemDataForId(itemId);
                }
            }
        }
    }

    public class DefaultTreeViewControllerWrap<T>
    {
        private MethodInfo _addItem;
        private object _defaultTreeViewController;
        private MethodInfo _getAllItemIds;
        private MethodInfo _getChildrenIds;
        private MethodInfo _getDataForId;
        private MethodInfo _getDataForIndex;
        private MethodInfo _getItemForIndex;
        private MethodInfo _getParentId;
        private MethodInfo _getTreeViewItemDataForId;
        private MethodInfo _getTreeViewItemDataForIndex;
        private MethodInfo _hasChildren;
        private MethodInfo _isChildOf;
        private PropertyInfo _itemSourceWrap;
        private MethodInfo _move;
        private MethodInfo _setRootItems;
        private MethodInfo _tryRemoveItem;

        public DefaultTreeViewControllerWrap(object defaultTreeViewController)
        {
            _defaultTreeViewController = defaultTreeViewController;
            Type type = typeof(TreeViewController).Assembly.GetType("DefaultTreeViewController");
            _itemSourceWrap = type.GetProperty("itemsSource");
        }

        public IList itemsSource
        {
            get {
                return (IList)_itemSourceWrap.GetValue(this);
            }
            set {
                _itemSourceWrap.SetValue(this, value);
            }
        }

        public void SetRootItems(IList<TreeViewItemData<T>> items)
        {
            _setRootItems?.Invoke(_defaultTreeViewController, new object[]
            {
                items
            });
        }

        public void AddItem(in TreeViewItemData<T> item, int parentId, int childIndex, bool rebuildTree = true)
        {
            _addItem?.Invoke(_defaultTreeViewController, new object[]
            {
                item, parentId, childIndex, rebuildTree
            });
        }

        public bool TryRemoveItem(int id, bool rebuildTree = true)
        {
            return (bool)_tryRemoveItem?.Invoke(_defaultTreeViewController, new object[]
            {
                id, rebuildTree
            });
        }

        public TreeViewItemData<T> GetTreeViewItemDataForId(int id)
        {
            return (TreeViewItemData<T>)_getTreeViewItemDataForId?.Invoke(_defaultTreeViewController, new object[]
            {
                id
            });
        }

        public TreeViewItemData<T> GetTreeViewItemDataForIndex(int index)
        {
            return (TreeViewItemData<T>)_getTreeViewItemDataForIndex?.Invoke(_defaultTreeViewController, new object[]
            {
                index
            });
        }

        public T GetDataForId(int id)
        {
            return (T)_getDataForId?.Invoke(_defaultTreeViewController, new object[]
            {
                id
            });
        }

        public T GetDataForIndex(int index)
        {
            return (T)_getDataForIndex?.Invoke(_defaultTreeViewController, new object[]
            {
                index
            });
        }

        public object GetItemForIndex(int index)
        {
            return _getItemForIndex?.Invoke(_defaultTreeViewController, new object[]
            {
                index
            });
        }

        public int GetParentId(int id)
        {
            return (int)_getParentId?.Invoke(_defaultTreeViewController, new object[]
            {
                id
            });
        }

        public bool HasChildren(int id)
        {
            return (bool)_hasChildren?.Invoke(_defaultTreeViewController, new object[]
            {
                id
            });
        }

        public IEnumerable<int> GetChildrenIds(int id)
        {
            return (IEnumerable<int>)_getChildrenIds?.Invoke(_defaultTreeViewController, new object[]
            {
                id
            });
        }

        public void Move(int id, int newParentId, int childIndex = -1, bool rebuildTree = true)
        {
            _setRootItems?.Invoke(_defaultTreeViewController, new object[]
            {
                id, newParentId, childIndex, rebuildTree
            });
        }

        private bool IsChildOf(int childId, int id)
        {
            return (bool)_setRootItems?.Invoke(_defaultTreeViewController, new object[]
            {
                childId, id
            });
        }

        public IEnumerable<int> GetAllItemIds(IEnumerable<int> rootIds = null)
        {
            return (IEnumerable<int>)_setRootItems?.Invoke(_defaultTreeViewController, new object[]
            {
                rootIds
            });
        }
    }
}