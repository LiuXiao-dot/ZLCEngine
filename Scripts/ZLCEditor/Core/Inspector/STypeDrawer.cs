using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using ZLCEditor.Inspector.VisualElements;
using ZLCEngine.SerializeTypes;
namespace ZLCEditor.Inspector
{
    [CustomPropertyDrawer(typeof(SType))]
    public class STypeInfoDrawer : PropertyDrawer
    {
        private static List<string> types;
        private static List<Type> realTypes;

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

        private class SearchMenu : IGenericMenu
        {
            private ListPopupContent _windowContent;
            private ZLCPopupWindow _window;

            public SearchMenu()
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
        }
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var choices = GetTypes();
            var defaultIndex = choices.IndexOf(property.structValue.ToString());
            var typeProperty = property.FindPropertyRelative("typeName");
            var assemblyProperty = property.FindPropertyRelative("assemblyName");
            defaultIndex = defaultIndex == -1 ? 0 : defaultIndex;
            var dropdown = new DropdownField(choices, defaultIndex);
            dropdown.createMenuCallback = () =>
            {
                var osMenu = new SearchMenu();
                /*for (int i = 0; i < choices.Count; i++) {
                    var option = choices[i];
                    var isValueSelected = i == dropdown.index;

                    osMenu.AddItem(option, isValueSelected, () =>
                    {
                        dropdown.value = option;
                    });
                }*/

                return osMenu;
            };
            dropdown.RegisterValueChangedCallback(s =>
            {
                var newIndex = dropdown.index;
                var newType = realTypes[newIndex];
                typeProperty.stringValue = newType.FullName;
                assemblyProperty.stringValue = newType.Assembly.FullName;
                property.serializedObject.ApplyModifiedProperties();
            });
            return dropdown;
        }

        private List<string> GetTypes()
        {
            if (types != null) return types;
            List<Type> ts = new List<Type>();
            realTypes = ts;
            EditorApplication.LockReloadAssemblies();
            try {
                EditorHelper.GetAllChildType<object>(ts, EditorHelper.AssemblyFilterType.Internal | EditorHelper.AssemblyFilterType.Custom);
                EditorHelper.GetAllInterfaces(ts, EditorHelper.AssemblyFilterType.Internal | EditorHelper.AssemblyFilterType.Custom);
                types = ts.Distinct().Select(t => t.FullName).ToList();
            }
            catch (Exception e) {
                Debug.LogError(e);
                throw;
            }
            finally {
                EditorApplication.UnlockReloadAssemblies();
            }
            return types;
        }
    }
}