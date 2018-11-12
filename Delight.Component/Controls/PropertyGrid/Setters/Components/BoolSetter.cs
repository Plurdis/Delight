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
    [Setter(Type = typeof(bool))]
    class BoolSetter : BaseSetter
    {
        ComboBox comboBox;

        public BoolSetter(object[] target, PropertyInfo[] pi) : base(target, pi)
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            comboBox = this.GetTemplateChild<ComboBox>("PART_valueComboBox");

            comboBox.Items.Add(true.ToString());
            comboBox.Items.Add(false.ToString());

            BindingHelper.SetBinding(
                this, ValueProperty,
                comboBox, ComboBox.SelectedItemProperty,
                converter: new BoolToStringConverter());
        }

        protected override void OnDispose()
        {
            if (comboBox == null)
                return;

            BindingOperations.ClearAllBindings(comboBox);

            comboBox = null;
        }
    }
}
