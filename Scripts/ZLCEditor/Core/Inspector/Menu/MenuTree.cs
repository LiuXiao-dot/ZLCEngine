using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEngine.UIElements;
using ZLCEngine.Utils;
namespace ZLCEditor.Inspector.Menu
{
    /// <summary>
    /// 菜单项
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
            return this.path;
        }

        public string GetName()
        {
            return Path.GetFileName(this.path);
        }
    }
    
    /// <summary>
    /// 菜单树
    /// </summary>
    public class MenuTree
    {
        /// <summary>
        /// 全部的item
        /// </summary>
        private List<TreeViewItemData<ZLCMenuItem>> allItems;
        /// <summary>
        /// 根节点item
        /// </summary>
        private List<TreeViewItemData<ZLCMenuItem>> rootItems;
        private int id = 0;
        
        // todo:搜索栏等
        public MenuTree()
        {
            allItems = new List<TreeViewItemData<ZLCMenuItem>>();
            rootItems = new List<TreeViewItemData<ZLCMenuItem>>();
        }
        public TreeViewItemData<ZLCMenuItem> Add(string path, object obj)
        {
            if (-1 != allItems.FindIndex(t => t.data.path == path)) return default;
            var isRoot = !(path.Contains('\\') || path.Contains('/'));
            var newItem = new TreeViewItemData<ZLCMenuItem>(id++,
                new ZLCMenuItem(path, obj), new List<TreeViewItemData<ZLCMenuItem>>());
            allItems.Add(newItem);
            if (isRoot) {
                rootItems.Add(newItem);
            } else {
                // 检测如果已有父item，直接添加上去，如果没有，则创建父item
                var index = allItems.FindIndex(t => FileHelper.IsFileInDirectory(t.data.path, path));
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