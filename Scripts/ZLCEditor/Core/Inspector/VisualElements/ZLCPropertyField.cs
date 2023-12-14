using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
namespace ZLCEditor.Inspector.VisualElements
{
    /// <summary>
    ///     <para>
    ///         ZLC的VisualElement中的基础UI控件
    ///     </para>
    /// </summary>
    public class ZLCPropertyField : PropertyField
    {

        /// <summary>
        ///        <para>
        /// PropertyField constructor.
        /// </para>
        ///      </summary>
        public ZLCPropertyField()
            : base((SerializedProperty)null, (string)null)
        {
        }

        /// <summary>
        ///        <para>
        /// PropertyField constructor.
        /// </para>
        ///      </summary>
        /// <param name="property">Providing a SerializedProperty in the construct just sets the bindingPath. You will still have to call Bind() on the PropertyField afterwards.</param>
        public ZLCPropertyField(SerializedProperty property)
            : base(property, (string)null)
        {
        }

        /// <summary>
        ///        <para>
        /// PropertyField constructor.
        /// </para>
        ///      </summary>
        /// <param name="property">Providing a SerializedProperty in the construct just sets the bindingPath. You will still have to call Bind() on the PropertyField afterwards.</param>
        /// <param name="label">Optionally overwrite the property label.</param>
        public ZLCPropertyField(SerializedProperty property, string label) : base(property, label)
        {
        }

        /// <summary>
        /// BoxGroup这种包含很多个元素的元素
        /// </summary>
        private VisualElement _childContainer;
        
        [EventInterest(new System.Type[] {typeof (SerializedPropertyBindEvent)})]
        protected override void ExecuteDefaultActionAtTarget(EventBase evt)
        {
            base.ExecuteDefaultActionAtTarget(evt);
            if (!(evt is SerializedPropertyBindEvent evt1))
                return;
            Reset(evt1.bindProperty);
            evt.StopPropagation();
        }

        public override void Reset(SerializedProperty newProperty)
        {
            string str = (string)null;
            SerializedPropertyType propertyType = newProperty.propertyType;
            if (propertyType == SerializedPropertyType.ManagedReference)
                str = newProperty.managedReferenceFullTypename;
            bool flag = true;
            if (this.m_SerializedObject != null && this.m_SerializedObject.m_NativeObjectPtr != IntPtr.Zero && this.m_SerializedObject.isValid && this.m_SerializedProperty != null && this.m_SerializedProperty.isValid && propertyType == this.m_SerializedProperty.propertyType)
                flag = propertyType == SerializedPropertyType.ManagedReference && str != this.m_SerializedPropertyReferenceTypeName;
            this.m_SerializedProperty = newProperty;
            this.m_SerializedPropertyReferenceTypeName = str;
            this.m_SerializedObject = newProperty.serializedObject;
            if (this.m_ChildField != null && !flag) {
                this.ResetDecoratorDrawers(ScriptAttributeUtility.GetHandler(this.m_SerializedProperty));
                VisualElement fieldFromProperty = this.CreateOrUpdateFieldFromProperty(newProperty, (object)this.m_ChildField);
                if (fieldFromProperty == this.m_ChildField) {
                    if(_childContainer != null)
                        _childContainer.Add(fieldFromProperty);
                    return;
                }
                this.m_ChildField.Unbind();
                int index = this.IndexOf(this.m_ChildField);
                if (index >= 0) {
                    this.m_ChildField.RemoveFromHierarchy();
                    this.m_ChildField = fieldFromProperty;
                    if (_childContainer == null) {
                        this.hierarchy.Insert(index, this.m_ChildField);
                    } else {
                        _childContainer.Add(m_ChildField);
                    }
                }
            } else {
                this.Clear();
                VisualElement childField = this.m_ChildField;
                if (childField != null)
                    childField.Unbind();
                this.m_ChildField = (VisualElement)null;
                this.m_DecoratorDrawersContainer = (VisualElement)null;
                if (this.m_SerializedProperty == null || !this.m_SerializedProperty.isValid)
                    return;
                this.ComputeNestingLevel();
                VisualElement visualElement = (VisualElement)null;
                PropertyHandler handler = ScriptAttributeUtility.GetHandler(this.m_SerializedProperty);
                using (handler.ApplyNestingContext(this.m_DrawNestingLevel)) {
                    if (handler.hasPropertyDrawer) {
                        handler.propertyDrawer.m_PreferredLabel = this.label ?? this.m_SerializedProperty.localizedDisplayName;
                        visualElement = handler.propertyDrawer.CreatePropertyGUI(this.m_SerializedProperty);
                        if (visualElement == null) {
                            visualElement = this.CreatePropertyIMGUIContainer();
                            this.m_imguiChildField = visualElement;
                        } else
                            this.RegisterPropertyChangesOnCustomDrawerElement(visualElement);
                    } else {
                        visualElement = this.CreateOrUpdateFieldFromProperty(this.m_SerializedProperty);
                        this.m_ChildField = visualElement;
                    }
                }
                this.ResetDecoratorDrawers(handler);
                if (visualElement != null) {
                    this.PropagateNestingLevel(visualElement);
                    if (_childContainer == null) {
                        this.hierarchy.Add(visualElement);
                    } else {
                        _childContainer.Add(visualElement);
                    }
                }
                if (this.m_SerializedProperty.propertyType != SerializedPropertyType.ManagedReference)
                    return;
                this.m_ChildField.TrackPropertyValue(this.m_SerializedProperty, (Action<SerializedProperty>)(e => this.Bind(this.m_SerializedProperty.serializedObject)));
            }
        }

        public override void ResetDecoratorDrawers(PropertyHandler handler)
        {
            List<DecoratorDrawer> decoratorDrawers = handler.decoratorDrawers;
            if (decoratorDrawers == null || decoratorDrawers.Count == 0 || m_DrawNestingLevel > 0) {
                if (m_DecoratorDrawersContainer == null)
                    return;
                Remove(m_DecoratorDrawersContainer);
                m_DecoratorDrawersContainer = null;
            } else {
                if (m_DecoratorDrawersContainer == null) {
                    m_DecoratorDrawersContainer = new VisualElement();
                    m_DecoratorDrawersContainer.AddToClassList(decoratorDrawersContainerClassName);
                    Insert(0, m_DecoratorDrawersContainer);
                } else
                    m_DecoratorDrawersContainer.Clear();
                foreach (DecoratorDrawer decoratorDrawer in decoratorDrawers) {
                    DecoratorDrawer decorator = decoratorDrawer;
                    VisualElement ve = decorator.CreatePropertyGUI();
                    if (ve == null) {
                        ve = new IMGUIContainer(() =>
                        {
                            Rect position = new Rect();
                            position.height = decorator.GetHeight();
                            position.width = resolvedStyle.width;
                            decorator.OnGUI(position);
                            ve.style.height = position.height;
                        });
                        ve.style.height = decorator.GetHeight();
                    }
                    m_DecoratorDrawersContainer.Add(ve);
                    switch (decorator) {
                        case BoxGroupDrawer:
                            _childContainer = ve;
                            break;
                    }
                }
            }
        }
    }
}