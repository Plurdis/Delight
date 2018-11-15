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

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            valueComboBox = GetTemplateChild<ComboBox>("PART_valueComboBox");

            if (this.IsStable)
                valueComboBox.SelectedItem = 0;


            ValueChanged(null, null);
            valueComboBox.SelectionChanged += ValueComboBox_SelectionChanged;
            ValueProperty.AddValueChanged(this, ValueChanged);
        }

        ByteToLightColorConverter converter = new ByteToLightColorConverter();

        private void ValueChanged(object sender, EventArgs e)
        {
            int convertedValue = converter.Convert((byte)Value, typeof(int), null, null);

            if ((int)valueComboBox.SelectedIndex != convertedValue)
                valueComboBox.SelectedIndex = convertedValue;
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
