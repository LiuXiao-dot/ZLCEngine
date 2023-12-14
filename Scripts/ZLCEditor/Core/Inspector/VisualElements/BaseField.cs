using UnityEngine.UIElements;
namespace ZLCEditor.Inspector.VisualElements
{
    /// <summary>
    ///     基础字段UI控件
    /// </summary>
    /// <typeparam name="TValueType"></typeparam>
    public abstract class BaseField<TValueType> : BindableElement, INotifyValueChanged<TValueType>, IMixedValueSupport
    {
        /// <summary>
        ///     缩进
        /// </summary>
        private const int indentPerLevel = 15;

        protected internal static readonly string mixedValueString = "\u2014";

        /// <summary>
        ///     label的宽度比例
        /// </summary>
        private static CustomStyleProperty<float> _labelWidthRatioProperty = new CustomStyleProperty<float>("--zlc-property-field-label-width-ratio");
        /// <summary>
        ///     label的边距
        /// </summary>
        private static CustomStyleProperty<float> _labelExtraPaddingProperty = new CustomStyleProperty<float>("--zlc-property-field-label-extra-padding");
        /// <summary>
        ///     label的最小宽度
        /// </summary>
        private static CustomStyleProperty<float> _labelBaseMinWidthProperty = new CustomStyleProperty<float>("--zlc-property-field-label-base-min-width");
        private float _labelBaseMinWidth;
        private float _labelExtraPadding;

        private float _labelWidthRatio;
        public bool showMixedValue { get; set; }




        public void SetValueWithoutNotify(TValueType newValue)
        {

        }
        public TValueType value { get; set; }
    }
}