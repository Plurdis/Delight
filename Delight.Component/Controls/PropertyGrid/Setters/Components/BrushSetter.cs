using Delight.Component.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;

namespace Delight.Component.Controls
{
    [TemplatePart(Name = "PART_colorCanvas", Type = typeof(ColorCanvas))]
    [Setter(Type = typeof(Brush))]
    class BrushSetter : BaseSetter
    {
        ColorCanvas colorCanvas;

        public BrushSetter(object[] targets, PropertyInfo[] pis) : base(targets, pis)
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.PropertyType == typeof(Brush))
            {
                colorCanvas = GetTemplateChild<ColorCanvas>("PART_colorCanvas");

                if (this.IsStable)
                    colorCanvas.SelectedColor = (this.Value as SolidColorBrush).Color;

                ValueProperty.AddValueChanged(this, Value_Changed);
                colorCanvas.SelectedColorChanged += ColorCanvas_SelectedColorChanged;
            }
            else
            {
                this.IsEnabled = false;
            }
        }

        private void Value_Changed(object sender, EventArgs e)
        {
            if (this.Value is SolidColorBrush brush)
                colorCanvas.SelectedColor = brush.Color;
        }

        private void ColorCanvas_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            this.Value = new SolidColorBrush(e.NewValue.Value);
        }

        protected override void OnDispose()
        {
            if (colorCanvas == null)
                return;

            ValueProperty.RemoveValueChanged(this, Value_Changed);
            colorCanvas.SelectedColorChanged -= ColorCanvas_SelectedColorChanged;

            colorCanvas = null;
        }
    }
}
