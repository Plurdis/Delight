using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Delight.Controls.Property
{

    [TemplatePart(Name = "checkBox", Type = typeof(CheckBox))]
    public class PropertyCheckBox : Control, INotifyPropertyChanged
    {
        public static DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(PropertyCheckBox));
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static DependencyProperty CheckBoxTextProperty = DependencyProperty.Register(nameof(CheckBoxText), typeof(string), typeof(PropertyCheckBox));
        public string CheckBoxText
        {
            get => (string)GetValue(CheckBoxTextProperty);
            set => SetValue(CheckBoxTextProperty, value);
        }

        public bool IsChecked
        {
            get => (bool)checkBox.GetValue(CheckBox.IsCheckedProperty);
            set => checkBox.SetValue(CheckBox.IsCheckedProperty, value);
        }

        CheckBox checkBox;

        public event PropertyChangedEventHandler PropertyChanged;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            checkBox = GetTemplateChild("checkBox") as CheckBox;
            checkBox.Checked += CheckBox_Checked;
            checkBox.Unchecked += CheckBox_Checked;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Checked"));
        }
    }
}
