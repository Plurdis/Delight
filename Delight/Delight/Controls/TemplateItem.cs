using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Delight.Controls
{
    public class TemplateItem : ListBoxItem
    {
        public TemplateItem()
        {
            this.Style = FindResource("TemplateItemStyle") as Style;
        }

        public static DependencyProperty DescriptionProperty = DependencyProperty.Register(nameof(Description), typeof(string), typeof(TemplateItem));
        public string Description
        {
            get => GetValue(DescriptionProperty) as string;
            set => SetValue(DescriptionProperty, value);
        }
    }
}
