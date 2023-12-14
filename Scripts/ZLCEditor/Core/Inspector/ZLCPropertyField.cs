/*using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
namespace ZLCEditor.Inspector
{
    public class ZLCPropertyField : PropertyField
    {
        public ZLCPropertyField()
            : base((SerializedProperty)null, (string)null)
        {
        }

        public ZLCPropertyField(SerializedProperty property) : base(property, (string)null)
        {

        }

        public ZLCPropertyField(SerializedProperty property, string label) : base(property, label)
        {

        }

        public override VisualElement CreateOrUpdateFieldFromProperty(SerializedProperty property, object originalField = null)
        {
            SerializedPropertyType propertyType = property.propertyType;
            if (EditorGUI.HasVisibleChildFields(property, true) && !property.isArray)
                return this.CreateFoldout(property, originalField);
            this.TrimChildrenContainerSize(0);
            this.m_ChildrenContainer = (VisualElement)null;
            switch (propertyType) {
                case SerializedPropertyType.Generic:
                    return property.isArray ? this.ConfigureListView(originalField as ListView, property, (Func<ListView>)(() => new ListView())) : (VisualElement)null;
                case SerializedPropertyType.Integer:
                    if (property.type == "long")
                        return (VisualElement)this.ConfigureField<LongField, long>(originalField as LongField, property, (Func<LongField>)(() => new LongField()));
                    if (property.type == "ulong")
                        return (VisualElement)this.ConfigureField<UnsignedLongField, ulong>(originalField as UnsignedLongField, property, (Func<UnsignedLongField>)(() => new UnsignedLongField()));
                    if (property.type == "uint")
                        return (VisualElement)this.ConfigureField<UnsignedIntegerField, uint>(originalField as UnsignedIntegerField, property, (Func<UnsignedIntegerField>)(() => new UnsignedIntegerField()));
                    IntegerField fieldFromProperty1 = this.ConfigureField<IntegerField, int>(originalField as IntegerField, property, (Func<IntegerField>)(() => new IntegerField()));
                    if (fieldFromProperty1 != null)
                        fieldFromProperty1.isDelayed = false;
                    return (VisualElement)fieldFromProperty1;
                case SerializedPropertyType.Boolean:
                    return (VisualElement)this.ConfigureField<Toggle, bool>(originalField as Toggle, property, (Func<Toggle>)(() => new Toggle()));
                case SerializedPropertyType.Float:
                    return property.type == "double" ? (VisualElement)this.ConfigureField<DoubleField, double>(originalField as DoubleField, property, (Func<DoubleField>)(() => new DoubleField())) : (VisualElement)this.ConfigureField<FloatField, float>(originalField as FloatField, property, (Func<FloatField>)(() => new FloatField()));
                case SerializedPropertyType.String:
                    TextField fieldFromProperty2 = this.ConfigureField<TextField, string>(originalField as TextField, property, (Func<TextField>)(() => new TextField()));
                    fieldFromProperty2.maxLength = -1;
                    return (VisualElement)fieldFromProperty2;
                case SerializedPropertyType.Color:
                    return (VisualElement)this.ConfigureField<ColorField, Color>(originalField as ColorField, property, (Func<ColorField>)(() => new ColorField()));
                case SerializedPropertyType.ObjectReference:
                    ObjectField fieldFromProperty3 = this.ConfigureField<ObjectField, UnityEngine.Object>(originalField as ObjectField, property, (Func<ObjectField>)(() => new ObjectField()));
                    System.Type type1 = (System.Type)null;
                    if (NativeClassExtensionUtilities.ExtendsANativeType(property.serializedObject.targetObject))
                        ScriptAttributeUtility.GetFieldInfoFromProperty(property, out type1);
                    if (type1 == (System.Type)null) {
                        string str = PropertyField.s_MatchPPtrTypeName.Match(property.type).Groups[1].Value;
                        foreach (System.Type type2 in TypeCache.GetTypesDerivedFrom<UnityEngine.Object>()) {
                            if (type2.Name.Equals(str, StringComparison.OrdinalIgnoreCase)) {
                                type1 = type2;
                                break;
                            }
                        }
                    }
                    fieldFromProperty3.SetProperty(ObjectField.serializedPropertyKey, (object)property);
                    fieldFromProperty3.SetObjectTypeWithoutDisplayUpdate(type1);
                    fieldFromProperty3.UpdateDisplay();
                    return (VisualElement)fieldFromProperty3;
                case SerializedPropertyType.LayerMask:
                    return (VisualElement)this.ConfigureField<LayerMaskField, int>(originalField as LayerMaskField, property, (Func<LayerMaskField>)(() => new LayerMaskField()));
                case SerializedPropertyType.Enum:
                    System.Type enumType;
                    ScriptAttributeUtility.GetFieldInfoFromProperty(property, out enumType);
                    if (enumType != (System.Type)null && enumType.IsDefined(typeof(FlagsAttribute), false)) {
                        EnumData enumData = UnityEditor.EnumDataUtility.GetCachedEnumData(enumType);
                        EnumFlagsField enumFlagsField = null;
                        int num;
                        if (originalField != null) {
                            enumFlagsField = originalField as EnumFlagsField;
                            num = enumFlagsField != null ? 1 : 0;
                        } else
                            num = 0;
                        if (num != 0) {
                            enumFlagsField.choices = ((IEnumerable<string>)enumData.displayNames).ToList<string>();
                            enumFlagsField.value = (Enum)Enum.ToObject(enumType, property.intValue);
                        }
                        return (VisualElement)this.ConfigureField<EnumFlagsField, Enum>(originalField as EnumFlagsField, property, (Func<EnumFlagsField>)(() =>
                        {
                            return new EnumFlagsField()
                            {
                                choices = ((IEnumerable<string>)enumData.displayNames).ToList<string>(),
                                value = (Enum)Enum.ToObject(enumType, property.intValue)
                            };
                        }));
                    }
                    EnumData? nullable = enumType != (System.Type)null ? new EnumData?(UnityEditor.EnumDataUtility.GetCachedEnumData(enumType)) : new EnumData?();
                    string[] enumDisplayNames = EditorGUI.EnumNamesCache.GetEnumDisplayNames(property);
                    List<string> popupEntries = ((IEnumerable<string>)(nullable?.displayNames ?? enumDisplayNames)).ToList<string>();
                    int propertyFieldIndex = property.enumValueIndex < 0 || property.enumValueIndex >= enumDisplayNames.Length ? -1 : (nullable.HasValue ? Array.IndexOf<string>(nullable.Value.displayNames, enumDisplayNames[property.enumValueIndex]) : property.enumValueIndex);
                    PopupField<string> popupField = null;
                    int num1;
                    if (originalField != null) {
                        popupField = originalField as PopupField<string>;
                        num1 = popupField != null ? 1 : 0;
                    } else
                        num1 = 0;
                    if (num1 != 0) {
                        popupField.choices = popupEntries;
                        popupField.index = propertyFieldIndex;
                    }
                    return (VisualElement)this.ConfigureField<PopupField<string>, string>(originalField as PopupField<string>, property, (Func<PopupField<string>>)(() => new PopupField<string>(popupEntries, property.enumValueIndex)
                    {
                        index = propertyFieldIndex
                    }));
                case SerializedPropertyType.Vector2:
                    return (VisualElement)this.ConfigureField<Vector2Field, Vector2>(originalField as Vector2Field, property, (Func<Vector2Field>)(() => new Vector2Field()));
                case SerializedPropertyType.Vector3:
                    return (VisualElement)this.ConfigureField<Vector3Field, Vector3>(originalField as Vector3Field, property, (Func<Vector3Field>)(() => new Vector3Field()));
                case SerializedPropertyType.Vector4:
                    return (VisualElement)this.ConfigureField<Vector4Field, Vector4>(originalField as Vector4Field, property, (Func<Vector4Field>)(() => new Vector4Field()));
                case SerializedPropertyType.Rect:
                    return (VisualElement)this.ConfigureField<RectField, Rect>(originalField as RectField, property, (Func<RectField>)(() => new RectField()));
                case SerializedPropertyType.ArraySize:
                    if (!(originalField is IntegerField integerField)) {
                        integerField = new IntegerField();
                        integerField.RegisterValueChangedCallback<int>((EventCallback<ChangeEvent<int>>)(evt => this.OnFieldValueChanged((EventBase)evt)));
                    }
                    integerField.SetValueWithoutNotify(property.intValue);
                    integerField.isDelayed = true;
                    return (VisualElement)this.ConfigureField<IntegerField, int>(integerField, property, (Func<IntegerField>)(() => new IntegerField()));
                case SerializedPropertyType.Character:
                    if (originalField is TextField field)
                        field.maxLength = 1;
                    else {
                        field = null;
                    }
                    return (VisualElement)this.ConfigureField<TextField, string>(field, property, (Func<TextField>)(() =>
                    {
                        return new TextField()
                        {
                            maxLength = 1
                        };
                    }));
                case SerializedPropertyType.AnimationCurve:
                    return (VisualElement)this.ConfigureField<CurveField, AnimationCurve>(originalField as CurveField, property, (Func<CurveField>)(() => new CurveField()));
                case SerializedPropertyType.Bounds:
                    return (VisualElement)this.ConfigureField<BoundsField, Bounds>(originalField as BoundsField, property, (Func<BoundsField>)(() => new BoundsField()));
                case SerializedPropertyType.Gradient:
                    return (VisualElement)this.ConfigureField<GradientField, Gradient>(originalField as GradientField, property, (Func<GradientField>)(() => new GradientField()));
                case SerializedPropertyType.Quaternion:
                    return (VisualElement)null;
                case SerializedPropertyType.ExposedReference:
                    return (VisualElement)null;
                case SerializedPropertyType.FixedBufferSize:
                    return (VisualElement)this.ConfigureField<IntegerField, int>(originalField as IntegerField, property, (Func<IntegerField>)(() => new IntegerField()));
                case SerializedPropertyType.Vector2Int:
                    return (VisualElement)this.ConfigureField<Vector2IntField, Vector2Int>(originalField as Vector2IntField, property, (Func<Vector2IntField>)(() => new Vector2IntField()));
                case SerializedPropertyType.Vector3Int:
                    return (VisualElement)this.ConfigureField<Vector3IntField, Vector3Int>(originalField as Vector3IntField, property, (Func<Vector3IntField>)(() => new Vector3IntField()));
                case SerializedPropertyType.RectInt:
                    return (VisualElement)this.ConfigureField<RectIntField, RectInt>(originalField as RectIntField, property, (Func<RectIntField>)(() => new RectIntField()));
                case SerializedPropertyType.BoundsInt:
                    return (VisualElement)this.ConfigureField<BoundsIntField, BoundsInt>(originalField as BoundsIntField, property, (Func<BoundsIntField>)(() => new BoundsIntField()));
                case SerializedPropertyType.Hash128:
                    return (VisualElement)this.ConfigureField<Hash128Field, Hash128>(originalField as Hash128Field, property, (Func<Hash128Field>)(() => new Hash128Field()));
                default:
                    return (VisualElement)null;
            }
        }
    }
}*/
