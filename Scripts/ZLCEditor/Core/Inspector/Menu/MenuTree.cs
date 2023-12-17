using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.UIElements;
using ZLCEngine.Utils;
namespace ZLCEditor.Inspector.Menu
{
    /// <summary>
    ///     菜单项
    /// </summary>
    public class ZLCMenuItem
    {
        public string path;
        public object target;

        public ZLCMenuItem(string path, object target)
        {
            this.path = path;
            this.target = target;
        }

        public string GetPath()
        {
            return path;
        }

        public string GetName()
        {
            return Path.GetFileName(path);
        }
    }

    /// <summary>
    ///     菜单树
    /// </summary>
    public class MenuTree
    {
        /// <summary>
        ///     全部的item
        /// </summary>
        private List<TreeViewItemData<ZLCMenuItem>> allItems;
        private int id;
        /// <summary>
        ///     根节点item
        /// </summary>
        private List<TreeViewItemData<ZLCMenuItem>> rootItems;

        // todo:搜索栏等
        public MenuTree()
        {
            allItems = new List<TreeViewItemData<ZLCMenuItem>>();
            rootItems = new List<TreeViewItemData<ZLCMenuItem>>();
        }
        public TreeViewItemData<ZLCMenuItem> Add(string path, object obj)
        {
            var oldIndex = allItems.FindIndex(t => t.data.path == path);
            if (-1 != oldIndex) {
                if (allItems[oldIndex].data.target == null) {
                    var oldItem = allItems[oldIndex];
                    var coverItem = new TreeViewItemData<ZLCMenuItem>(id++,
                        new ZLCMenuItem(path, obj), (List<TreeViewItemData<ZLCMenuItem>>)oldItem.children);
                    allItems[oldIndex] = coverItem;
                    
                    var oldRootIndex = rootItems.FindIndex(t => t.data.path == path);
                    if (oldRootIndex != -1) {
                        rootItems[oldRootIndex] = coverItem;
                    }
                }
                return allItems[oldIndex];
            }
            bool isRoot = !(path.Contains('\\') || path.Contains('/'));
            TreeViewItemData<ZLCMenuItem> newItem = new TreeViewItemData<ZLCMenuItem>(id++,
                new ZLCMenuItem(path, obj), new List<TreeViewItemData<ZLCMenuItem>>());
            allItems.Add(newItem);
            if (isRoot) {
                rootItems.Add(newItem);
            } else {
                // 检测如果已有父item，直接添加上去，如果没有，则创建父item
                int index = allItems.FindIndex(t => FileHelper.IsFileInDirectory(t.data.path, path));
                TreeViewItemData<ZLCMenuItem> parentItem = index == -1 ? Add(Path.GetDirectoryName(path), null) : allItems[index];
                ((List<TreeViewItemData<ZLCMenuItem>>)parentItem.children).Add(newItem);
            }
            return newItem;
        }

        public void SortMenuItemsByName()
        {
        }

        public List<TreeViewItemData<ZLCMenuItem>> GetItems()
        {
            return rootItems;
        }
    }
}