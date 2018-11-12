using Delight.Component.Converters;
using Delight.Component.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Delight.Component.Controls
{
    [TemplatePart(Name = "PART_valueComboBox", Type = typeof(ComboBox))]
    [Setter(Key = "LightColor", Type = typeof(byte))]
    public class LightColorSetter : BaseSetter
    {
        ComboBox valueComboBox;

        public LightColorSetter(object[] target, PropertyInfo[] pi) : base(target, pi)
        {
        }

        // TODO: 완성
        // TODO: 아이템들을 직접 추가할 것

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            valueComboBox = GetTemplateChild<ComboBox>("PART_valueComboBox");

            if (this.IsStable)
                valueComboBox.SelectedItem = 0;

            valueComboBox.SelectionChanged += ValueComboBox_SelectionChanged;

            ValueProperty.AddValueChanged(this, ValueChanged);
        }

        ByteToLightColorConverter converter;

        private void ValueChanged(object sender, EventArgs e)
        {
            int convertedValue = converter.Convert((byte)Value, typeof(int), null, null);

            if (valueComboBox.SelectedIndex != convertedValue)
                valueComboBox.SelectedItem = convertedValue;
        }

        private void ValueComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            byte convertedValue = converter.ConvertBack(valueComboBox.SelectedIndex, typeof(byte), null, null);

            if ((byte)Value != convertedValue)
                Value = convertedValue;
        }

        protected override void OnDispose()
        {
            if (valueComboBox == null)
                return;

            valueComboBox.SelectionChanged -= ValueComboBox_SelectionChanged;
            ValueProperty.RemoveValueChanged(this, ValueChanged);

            valueComboBox = null;
        }
    }
}
