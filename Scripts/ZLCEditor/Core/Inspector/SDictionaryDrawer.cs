using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using ZLCEditor.Inspector.VisualElements;
using ZLCEngine.SerializeTypes;
namespace ZLCEditor.Inspector
{
    [CustomPropertyDrawer(typeof(SDictionary<,>))]
    public class SDictionaryDrawer : PropertyDrawer
    {
        static readonly string listViewNamePrefix = "unity-list-";
        internal static readonly string listViewBoundFieldProperty = "unity-list-view-property-field-bound";

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return ConfigureListView(null, property, ()=>new ZLCListView());
        }
        
        VisualElement ConfigureListView(ListView listView, SerializedProperty property, Func<ListView> factory)
        {
            var realValue = property.FindPropertyRelative("_cache");
            if (listView == null)
            {
                listView = factory();
                listView.showBorder = true;
                listView.selectionType = SelectionType.Multiple;
                listView.showAddRemoveFooter = true;
                listView.showBoundCollectionSize = true;
                listView.showFoldoutHeader = true;
                listView.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
                listView.showAlternatingRowBackgrounds = AlternatingRowBackground.None;
                listView.itemsSourceSizeChanged += ()=> DispatchPropertyChangedEvent(property, listView);
            }
            var kvType = fieldInfo.FieldType;
            var kvRealTypes = kvType.GetGenericArguments();
            listView.SetViewController(new SDictionartController(kvRealTypes[0], kvRealTypes[1]));

            var propertyCopy = realValue.Copy();
            var listViewName = $"{listViewNamePrefix}{realValue.propertyPath}";
            //listView.headerTitle = string.IsNullOrEmpty(label) ? propertyCopy.localizedDisplayName : label;
            listView.userData = propertyCopy;
            listView.bindingPath = realValue.propertyPath;
            listView.viewDataKey = listViewName;
            listView.name = listViewName;
            listView.SetProperty(listViewBoundFieldProperty, this);

            // Make list view foldout react even when disabled, like EditorGUILayout.Foldout.
            var toggle = listView.Q<Toggle>(className: Foldout.toggleUssClassName);
            if (toggle != null)
                toggle.m_Clickable.acceptClicksIfDisabled = true;

            return listView;
        }
        
        private void DispatchPropertyChangedEvent(SerializedProperty serializedProperty, VisualElement target)
        {
            using (var evt = SerializedPropertyChangeEvent.GetPooled(serializedProperty))
            {
                evt.target = target;
                (target.panel as BaseVisualElementPanel)?.SendEvent(evt);
            }
        }
    }

    public class SDictionartController : BaseListViewController
    {
        protected ListView listView => view as ListView;
        private Type _key;
        private Type _value;

        public SDictionartController(Type key, Type value)
        {
            this._key = key;
            this._value = value;
        }
        
        protected override VisualElement MakeItem()
        {
            if (listView.makeItem == null)
            {
                if (listView.bindItem != null)
                    throw new NotImplementedException("You must specify makeItem if bindItem is specified.");
                return new Label();
            }
            return listView.makeItem.Invoke();
        }
        protected override void BindItem(VisualElement element, int index)
        {
            if (listView.bindItem == null)
            {
                if (listView.makeItem != null)
                    throw new NotImplementedException("You must specify bindItem if makeItem is specified.");

                var label = (Label)element;
                var item = listView.itemsSource[index];
                label.text = item?.ToString() ?? "null";
                return;
            }

            listView.bindItem.Invoke(element, index);
        }
        protected override void UnbindItem(VisualElement element, int index)
        {
            listView.unbindItem?.Invoke(element, index);
        }
        protected override void DestroyItem(VisualElement element)
        {
            listView.destroyItem?.Invoke(element);
        }


        public override void AddItems(int itemCount)
        {
            if (itemCount > 1) itemCount = 1;
            ZLCPopupWindow.Show(listView.worldBound, new TempDicAddValuePanel(_key, _value));
        }
        
        private class TempDicAddValuePanel : ZLCPopupWindowContent
        {
            private Type _key;
            private Type _value;
            
            public TempDicAddValuePanel(Type key, Type value)
            {
                this._key = key;
                this._value = value;
            }
            
            public override VisualElement CreateUI()
            {
                var root = new VisualElement();

                var tempKey = Activator.CreateInstance(_key);
                var tempValue = Activator.CreateInstance(_value);
                root.Add(ZLCDrawerHelper.CreateDrawer(tempKey));
                root.Add(ZLCDrawerHelper.CreateDrawer(tempValue));
                var btn = new Button();
                btn.text = "添加";
                root.Add(btn);
                
                return root;
            }
        }
        
        private VisualElement CreateAddItemPanel()
        {
            
            
            return null;
        }
    }
}