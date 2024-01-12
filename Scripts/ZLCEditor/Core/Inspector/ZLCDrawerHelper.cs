using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using ZLCEditor.Inspector.VisualElements;
using ZLCEngine.Inspector;
namespace ZLCEditor.Inspector
{
    /// <summary>
    /// ZLC编辑器帮助类
    /// </summary>
    public class ZLCDrawerHelper
    {
        private static Dictionary<Type, Type> _drawers;

        /// <summary>
        /// 初始化_drawers
        /// </summary>
        private static void InitDrawers()
        {
            var temps = new List<Type>();
            EditorHelper.GetAllChildType<ZLCDrawer>(temps, EditorHelper.AssemblyFilterType.Internal | EditorHelper.AssemblyFilterType.Custom);
            _drawers = new Dictionary<Type, Type>();
            foreach (var temp in temps) {
                var attr = temp.GetCustomAttribute<CustomPropertyDrawer>();
                if (attr == null) continue;
                _drawers.Add(attr.m_Type, temp);
            }
        }

        internal static Type GetDrawerTypeForType(Type type)
        {
            if (_drawers == null) InitDrawers();
            Type drawerType;
            _drawers.TryGetValue(type, out drawerType);
            if (drawerType != null)
                return drawerType;

            //
            // 忽略泛型参数
            if (type.IsGenericType)
                _drawers.TryGetValue(type.GetGenericTypeDefinition(), out drawerType);

            if (drawerType != null) return drawerType;
            
            if (!type.IsValueType) {
                for (Type currentType = type; currentType != null; currentType = currentType.BaseType) {
                    _drawers.TryGetValue(currentType, out drawerType);
                    if(drawerType == null) continue;
                    var attr = drawerType.GetCustomAttribute<CustomPropertyDrawer>();
                    if(attr == null) continue;
                    if(!attr.m_UseForChildren) continue;
                    _drawers.Add(type, drawerType);
                    return drawerType;
                }
            }
            return null;
        }

        public static Type GetDrawerType(object obj)
        {
            var type = obj.GetType();
            return GetDrawerTypeForType(type);
        }

        /// <summary>
        /// Editor的UI
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static VisualElement CreateEditorGUI(object obj)
        {
            if (obj is SerializedProperty serializedProperty) return new ZLCPropertyField(serializedProperty);
  
            SerializedObject serializedObj = null;
            if (obj is UnityEngine.Object unityObj) {
                var editorType = CustomEditorAttributes.FindCustomEditorTypeByType(obj.GetType(), false);
                if (editorType != null) {
                    return new InspectorElement(unityObj);
                }
                serializedObj = new SerializedObject(unityObj);
            }

            if (obj is SerializedObject) {
                serializedObj = (SerializedObject)obj;
            }
            if (serializedObj != null) {
                return CreateSerializedObjectEditorUI(serializedObj);
            }

            return CreateSystemObjectEditorUI(obj);
        }

        private static VisualElement CreateSerializedObjectEditorUI(SerializedObject sObject)
        {
            var root = new VisualElement();
            var value = sObject.targetObject;
            var members = EditorHelper.SortByMetadataTokenOrder(value.GetType());
            //var iterator = sObject.GetIterator();

            foreach (var memberInfo in members) {
                if (memberInfo is FieldInfo fieldInfo) {
                    if (memberInfo.GetCustomAttribute<HideInInspector>() != null) {
                        continue;
                    }
                    var fieldProperty = sObject.FindProperty(fieldInfo.Name);
                    var prop = new ZLCPropertyField(fieldProperty);
                    prop.Bind(sObject);
                    root.Add(prop);
                    continue;
                }

                // -- 检测各个方法是否有被可序列化的特性，如果有则按对应的特性进行序列化，没有则跳过
                if (memberInfo is MethodInfo methodInfo) {
                    AnySerializableAttribute attribute = methodInfo.GetCustomAttribute<AnySerializableAttribute>(true);
                    if (attribute == null) continue;
                    Type drawerType = ScriptAttributeUtility.GetDrawerTypeForType(attribute.GetType());
                    if (drawerType != null) {
                        IAnySerializableAttributeEditor drawer = (IAnySerializableAttributeEditor)Activator.CreateInstance(drawerType);
                        VisualElement methodGUI = drawer.CreateGUI(attribute, methodInfo, value);
                        if (methodGUI != null)
                            root.Add(methodGUI);
                    }
                }
            }
            return root;
        }

        private static VisualElement CreateSystemObjectEditorUI(object value)
        {
            if (value == null) return null;
            var root = new VisualElement();
            var members = EditorHelper.SortByMetadataTokenOrder(value.GetType());
            var tempSo = ScriptableObject.CreateInstance<ZLCTempObject>();
            tempSo.t = value;
            var serilizedObejct = new SerializedObject(tempSo);
            var rootProperty = serilizedObejct.FindProperty("t");

            foreach (var memberInfo in members) {
                if (memberInfo is FieldInfo fieldInfo) {
                    using (new EditorGUI.DisabledScope(false)) {
                        //var field = fieldInfo.GetValue(value);
                        var drawerType = ZLCDrawerHelper.GetDrawerTypeForType(fieldInfo.FieldType);
                        if (drawerType != null) {
                            var drawer = Activator.CreateInstance(drawerType, rootProperty.FindPropertyRelative(fieldInfo.Name)) as ZLCDrawer;
                            root.Add(drawer.CreateGUI());
                        } else {
                            root.Add(new PropertyField(rootProperty.FindPropertyRelative(fieldInfo.Name)));
                        }
                    }
                    continue;
                }

                // -- 检测各个方法是否有被可序列化的特性，如果有则按对应的特性进行序列化，没有则跳过
                if (memberInfo is MethodInfo methodInfo) {
                    AnySerializableAttribute attribute = methodInfo.GetCustomAttribute<AnySerializableAttribute>(true);
                    if (attribute == null) continue;
                    Type drawerType = ScriptAttributeUtility.GetDrawerTypeForType(attribute.GetType());
                    if (drawerType != null) {
                        IAnySerializableAttributeEditor drawer = (IAnySerializableAttributeEditor)Activator.CreateInstance(drawerType);
                        VisualElement methodGUI = drawer.CreateGUI(attribute, methodInfo, value);
                        if (methodGUI != null)
                            root.Add(methodGUI);
                    }
                }
            }
            root.Bind(serilizedObejct);
            return root;
        }

        /// <summary>
        /// 创建Drawer
        /// </summary>
        /// <returns></returns>
        public static VisualElement CreateDrawer(object obj)
        {
            if (obj is SerializedProperty serializedProperty) {
                /*switch (serializedProperty.propertyType) {
                    case SerializedPropertyType.ManagedReference:
                    {
                        var value = serializedProperty.managedReferenceValue;
                        // 非Object
                        
                    }
                        break;
                    case SerializedPropertyType.ObjectReference:
                    {
                        var value = serializedProperty.objectReferenceValue;
                        
                    }
                        break;
                }*/
                return new ZLCPropertyField(serializedProperty);
            }

            
            return null;
        }
    }
}