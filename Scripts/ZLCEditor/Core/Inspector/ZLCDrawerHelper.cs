using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using ZLCEditor.Inspector.VisualElements;
using ZLCEngine.Inspector;
using Object = UnityEngine.Object;
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
        public static VisualElement CreateEditorGUI(object obj, Type type = null)
        {
            SerializedObject serializedObj = null;
            if (obj is UnityEngine.Object unityObj) {
                var editorType = CustomEditorAttributes.FindCustomEditorTypeByType(obj.GetType(), false);
                if (editorType != null && editorType != typeof(BaseZLCEditor) && !editorType.IsSubclassOf(typeof(BaseZLCEditor))) {
                    var editor = Editor.CreateEditor(unityObj, editorType);
                    return editor.CreateInspectorGUI();
                }
                serializedObj = new SerializedObject(unityObj);
            }

            if (obj is SerializedObject so) {
                // 检测是否有自定义编辑器
                var editorType = CustomEditorAttributes.FindCustomEditorType(so.targetObject, false);
                if (editorType != null && editorType != typeof(BaseZLCEditor) && !editorType.IsSubclassOf(typeof(BaseZLCEditor))) {
                    var editor = Editor.CreateEditor(so.targetObject, editorType);
                    return editor.CreateInspectorGUI();
                }
                serializedObj = (SerializedObject)obj;
            }
            if (serializedObj != null) {
                /*if (serializedObj.targetObject is MeshFilter) {
                    return CreateDefaultEditor(serializedObj);
                }*/
                return CreateSerializedObjectEditorUI(serializedObj);
            }

            return CreateSystemObjectEditorUI(obj,out _, type);
        }

        private static VisualElement CreateDefaultEditor(SerializedObject obj)
        {
            var root = new VisualElement();
            // 添加默认的ui
            var iterator = obj.GetIterator();
            for (bool enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false)
            {
                using (new EditorGUI.DisabledScope("m_Script" == iterator.propertyPath))
                    root.Add(new PropertyField(iterator));
            }
            root.Bind(obj);
            return root;
        }

        private static VisualElement CreateSerializedObjectEditorUI(SerializedObject sObject)
        {
            var root = new VisualElement();
            var value = sObject.targetObject;
            var members = EditorHelper.SortByMetadataTokenOrder(value.GetType());
            var iterator = sObject.GetIterator();
            // 添加默认的ui,添加完后，再在member中找剩余可添加的
            for (bool enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false)
            {
                using (new EditorGUI.DisabledScope("m_Script" == iterator.propertyPath)) {
                    root.Add(new ZLCPropertyField(iterator));
                }
            }

            foreach (var memberInfo in members) {
                /*if (memberInfo is FieldInfo fieldInfo) {
                    if (memberInfo.GetCustomAttribute<HideInInspector>() != null) {
                        continue;
                    }
                    var fieldProperty = sObject.FindProperty(fieldInfo.Name);
                    var prop = new ZLCPropertyField(fieldProperty);
                    prop.Bind(sObject);
                    root.Add(prop);
                    continue;
                }*/

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
            root.Bind(sObject);
            return root;
        }

        private static VisualElement CreateSystemObjectEditorUI(object value, out SerializedObject serializedObject, Type type = null)
        {
            serializedObject = null;
            if (value == null) return null;
            var root = new VisualElement();
            var realType = type ?? value.GetType();
            var members = EditorHelper.SortByMetadataTokenOrder(realType);
            var tempSo = ZLCTempManager.CreateTemp(value, realType);
            serializedObject = new SerializedObject(tempSo);
            var rootProperty = serializedObject.FindProperty("value");
            switch (rootProperty.propertyType) {
                case SerializedPropertyType.Integer:
                case SerializedPropertyType.Boolean:
                case SerializedPropertyType.Float:
                case SerializedPropertyType.String:
                case SerializedPropertyType.Color:
                case SerializedPropertyType.LayerMask:
                case SerializedPropertyType.Enum:
                case SerializedPropertyType.Vector2:
                case SerializedPropertyType.Vector3:
                case SerializedPropertyType.Vector4:
                case SerializedPropertyType.Rect:
                case SerializedPropertyType.ArraySize:
                case SerializedPropertyType.Character:
                case SerializedPropertyType.AnimationCurve:
                case SerializedPropertyType.Bounds:
                case SerializedPropertyType.Gradient:
                case SerializedPropertyType.Quaternion:
                case SerializedPropertyType.FixedBufferSize:
                case SerializedPropertyType.Vector2Int:
                case SerializedPropertyType.Vector3Int:
                case SerializedPropertyType.RectInt:
                case SerializedPropertyType.BoundsInt:
                case SerializedPropertyType.Hash128:
                    root.Add(new ZLCPropertyField(rootProperty));
                    break;
                default:
                    foreach (var memberInfo in members) {
                        if (memberInfo is FieldInfo fieldInfo) {
                            using (new EditorGUI.DisabledScope(false)) {
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
                    break;
            }
            
            root.Bind(serializedObject);
            return root;
        }

        /// <summary>
        /// 创建Drawer
        /// </summary>
        /// <returns></returns>
        public static VisualElement CreateDrawer(object obj, out SerializedObject serializedObject,Type type = null)
        {
            serializedObject = null;
            if (obj is SerializedProperty serializedProperty) {
                serializedObject = serializedProperty.serializedObject;
                return new ZLCPropertyField(serializedProperty);
            }
            if (obj is Object unityObj) {
                return new InspectorElement(unityObj);
            }
            if (obj is System.Object systemObj) {
                return CreateSystemObjectEditorUI(systemObj, out serializedObject, type);
            }

            return null;
        }

        /// <summary>
        /// 创建自定义的或者默认的Drawer
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        private static VisualElement CreateDefaultDrawer(SerializedProperty property)
        {
            return null;
        }
    }
}