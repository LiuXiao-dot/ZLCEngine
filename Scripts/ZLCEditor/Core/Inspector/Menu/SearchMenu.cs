using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using ZLCEditor.Inspector.VisualElements;
namespace ZLCEditor.Inspector.Menu
{
    /// <summary>
    /// 查找窗口
    /// </summary>
    public class SearchMenu : IGenericMenu
    {
        public static VisualElement CreateDropdownField(List<string> choices, EventCallback<int,ChangeEvent<string>> callback, int defaultIndex = 0)
        {
            var dropdown = new DropdownField(choices, defaultIndex);
            dropdown.createMenuCallback = () =>
            {
                return new SearchMenu();;
            };
            dropdown.RegisterValueChangedCallback(s =>
            {
                callback?.Invoke(dropdown.index, s);
            });
            return dropdown;
        }

        public static VisualElement CreateButton(List<string> choices, EventCallback<int> callback, int defaultIndex = 0, VectorImage icon = null, bool showText = true)
        {
            var btn = new Button();
            btn.RegisterCallback<ClickEvent>(e =>
            {
                SearchMenu menu = new SearchMenu();
                var length = choices.Count;
                for (int i = 0; i < length; i++) {
                    var index = i;
                    var item = choices[index];
                    bool isSelected = EqualityComparer<string>.Default.Equals(item, btn.text);
                    menu.AddItem(item, isSelected, ()=>
                    {
                        menu._selected = item;
                        if(showText)
                            btn.text = item;
                        callback?.Invoke(index);
                    });
                }
                menu.DropDown(btn.worldBound, btn, true);
            });
            if (showText) {
                btn.text = choices[defaultIndex];
            } else {
                btn.AddToClassList("zlc-button-icon");
            }
            if (icon != null) {
                btn.style.backgroundImage = new StyleBackground(icon);
            }
            return btn;
        }
        
        private ListPopupContent _windowContent;
        private ZLCPopupWindow _window;
        private string _selected;
        
        private SearchMenu()
        {
            _windowContent = new ListPopupContent();
        }

        public void AddItem(string itemName, bool isChecked, System.Action action)
        {
            if (action == null)
                _windowContent.AddItem(itemName, null);
            else
                _windowContent.AddItem(itemName, action.Invoke);
        }
        public void AddItem(string itemName, bool isChecked, Action<object> action, object data)
        {
                
        }
        public void AddDisabledItem(string itemName, bool isChecked)
        {
        }
        public void AddSeparator(string path)
        {
        }
        public void DropDown(Rect position, VisualElement targetElement = null, bool anchored = false)
        {    
            _window = ZLCPopupWindow.Show(position, _windowContent);
        }
        
        private class ListPopupContent : ZLCPopupWindowContent
        {
            private ListView _listView;
            private List<ListItem> _menuItems;
            private ToolbarSearchField _searchField;
            private VisualElement _root;

            private List<ListItem> _showedItems;

            public ListPopupContent()
            {
                _menuItems = new List<ListItem>();
                _showedItems = new List<ListItem>();
                _root = new VisualElement();
                _root.AddToClassList("zlc-vertical");
                
                _searchField = new ToolbarSearchField();
                _searchField.value = "查找";
                _searchField.RegisterValueChangedCallback(t =>
                {
                    _showedItems.Clear();
                    var value = t.newValue;
                    var lower = value.ToLower();
                    var length = _menuItems.Count;
                    for (int i = 0; i < length; i++) {
                        if (_menuItems[i].content.ToLower().Contains(lower)) {
                            _showedItems.Add(_menuItems[i]);
                        }
                    }
                    _listView.Rebuild();
                });
                
                _listView = new ListView(_showedItems);
                _listView.AddToClassList("zlc-list-view");
                _listView.bindItem = (element, i) =>
                {
                    ((Label)element).text = _showedItems[i].content.ToString();
                };
                _listView.makeItem = () =>
                {
                    return new Label();
                };
                _listView.selectionChanged += OnSelectionChanged;
                _root.Add(_searchField);
                _root.Add(_listView);
            }
            
            private class ListItem
            {
                public string content;
                public Action selected;
                public bool show = true;
            }

            public override VisualElement CreateUI()
            {
                return _root;
            }
            private void OnSelectionChanged(IEnumerable<object> newSelect)
            {
                ((ListItem)newSelect.First()).selected?.Invoke();
            }
            public void AddItem(string content, Action selected)
            {
                _menuItems.Add(new ListItem()
                {
                    content = content,
                    selected = selected
                });
                _listView.RefreshItems();
            }
        }

    }
}