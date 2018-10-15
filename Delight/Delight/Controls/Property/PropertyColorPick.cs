using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;

namespace Delight.Controls.Property
{
    [TemplatePart(Name = "picker", Type = typeof(ColorPicker))]
    public class PropertyColorPick : Control, INotifyPropertyChanged
    {
        public static DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(PropertyColorPick));
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public Color SelectedColor
        {
            get => (Color)colorPicker.GetValue(ColorPicker.SelectedColorProperty);
            set => colorPicker.SetValue(ColorPicker.SelectedColorProperty, value);
        }

        ColorPicker colorPicker;

        public event PropertyChangedEventHandler PropertyChanged;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            colorPicker = GetTemplateChild("picker") as ColorPicker;

            colorPicker.SelectedColor = Color.FromArgb(255, 0, 0, 0);

            colorPicker.UsingAlphaChannel = false;
            colorPicker.ColorMode = ColorMode.ColorCanvas;

            colorPicker.SelectedColorChanged += ColorPicker_SelectedColorChanged;
        }

        private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs("SelectedColor"));
        }

        public PropertyColorPick()
        {
            ApplyTemplate();
        }
    }
}
