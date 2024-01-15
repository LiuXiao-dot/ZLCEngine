using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.UIElements.Bindings;
using UnityEngine;
using UnityEngine.Pool;
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
            var realProperty = property.FindPropertyRelative("_cache");
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
            
            var propertyCopy = realProperty.Copy();
            var listViewName = $"{listViewNamePrefix}{realProperty.propertyPath}";
            //listView.headerTitle = string.IsNullOrEmpty(label) ? propertyCopy.localizedDisplayName : label;
            listView.userData = propertyCopy;
            listView.bindingPath = realProperty.propertyPath;
            listView.viewDataKey = listViewName;
            listView.name = listViewName;
            listView.SetProperty(listViewBoundFieldProperty, this);

            // Make list view foldout react even when disabled, like EditorGUILayout.Foldout.
            var toggle = listView.Q<Toggle>(className: Foldout.toggleUssClassName);
            if (toggle != null)
                toggle.m_Clickable.acceptClicksIfDisabled = true;

            var kvType = fieldInfo.FieldType;
            var kvRealTypes = kvType.GetGenericArguments();
            var targetObject = property.serializedObject.targetObject;
            var realObj = fieldInfo.GetValue(targetObject);
            
            listView.SetViewController(new SDictionartController(kvRealTypes[0], kvRealTypes[1], fieldInfo, realObj));
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

    internal class SDictionartController : EditorListViewController
    {
        private Type _key;
        private Type _value;
        private FieldInfo _fieldInfo;// SDictionary
        private object _obj;// SDictionary

        public SDictionartController(Type key, Type value, FieldInfo fieldInfo, object realObj)
        {
            this._key = key;
            this._value = value;
            this._fieldInfo = fieldInfo;
            this._obj = realObj;
        }

        public override void AddItems(int itemCount)
        {
            if (itemCount > 1) itemCount = 1;
            ZLCPopupWindow.Show(listView.worldBound, new TempDicAddValuePanel(_key, _value, this));
        }

        /// <summary>
        /// 实际的添加Item操作
        /// </summary>
        private void AddItemBase(object tempKey, object tempValue)
        {
            var itemCount = 1;
            var previousCount = GetItemsCount();

            var kvType = typeof(SDictionary<,>.KV).MakeGenericType(_key, _value);
            var kv = Activator.CreateInstance(kvType);
            var keyFiled = kvType.GetField("key");
            var valueFiled = kvType.GetField("value");
            
            keyFiled.SetValue(kv,tempKey);
            valueFiled.SetValue(kv,tempValue);
            
            var index = serializedObjectList.ArrayProperty.arraySize;
            var type = _fieldInfo.FieldType;
            var cacheField = type.GetField("_cache", BindingFlags.Instance | BindingFlags.NonPublic);
            var currentValue = cacheField.GetValue(_obj) as Array;
            var newArray = Array.CreateInstance(cacheField.FieldType.GetElementType(), currentValue.Length + 1);
            Array.Copy(currentValue, newArray, currentValue.Length);
            newArray.SetValue(kv,index);
            cacheField.SetValue(_obj, newArray);
            ((ISerializationCallbackReceiver)_obj).OnAfterDeserialize();

            EditorUtility.SetDirty(serializedObjectList.ArrayProperty.serializedObject.targetObject);
            serializedObjectList.ArrayProperty.serializedObject.ApplyModifiedProperties();

            var indices = ListPool<int>.Get();
            try
            {
                for (var i = 0; i < itemCount; i++)
                {
                    indices.Add(previousCount + i);
                }

                RaiseItemsAdded(indices);
            }
            finally
            {
                ListPool<int>.Release(indices);
            }

            RaiseOnSizeChanged();
        }
        
        private class TempDicAddValuePanel : ZLCPopupWindowContent
        {
            private Type _key;
            private Type _value;
            private SDictionartController _controller;
            
            public TempDicAddValuePanel(Type key, Type value, SDictionartController controller)
            {
                this._key = key;
                this._value = value;
                this._controller = controller;
            }
            
            public override VisualElement CreateUI()
            {
                var root = new VisualElement();

                var tempKey = Activator.CreateInstance(_key);
                var tempValue = Activator.CreateInstance(_value);
                var keyVE = ZLCDrawerHelper.CreateDrawer(tempKey, out var keySO);
                var valueVE = ZLCDrawerHelper.CreateDrawer(tempValue, out var valueSO);
                
                root.Add(keyVE);
                root.Add(valueVE);
                var btn = new Button();
                btn.text = "添加";
                root.Add(btn);
                btn.RegisterCallback<ClickEvent>(e =>
                {
                    if (CheckAddItem()) {
                        _controller.AddItemBase(ZLCTempManager.GetTempValue(keySO.targetObject), ZLCTempManager.GetTempValue(valueSO.targetObject));;
                    }
                    ((ZLCPopupWindow)editorWindow).Close();
                });
                
                return root;
            }


            private bool CheckAddItem()
            {
                return true;
            }
        }
        
    }
}