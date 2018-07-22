using Delight.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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

        public static DependencyProperty SourceProperty = DependencyProperty.Register(nameof(Source), typeof(ImageSource), typeof(TemplateItem));
        public ImageSource Source
        {
            get => GetValue(SourceProperty) as ImageSource;
            set => SetValue(SourceProperty, value);
        }

    }
}
