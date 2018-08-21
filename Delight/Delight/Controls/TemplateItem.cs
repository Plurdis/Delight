using Delight.Common;
using Delight.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        [Category("UI적 요소")]
        public static DependencyProperty ItemNameProperty = DependencyProperty.Register(nameof(ItemName), typeof(string), typeof(TemplateItem));
        public string ItemName
        {
            get => GetValue(ItemNameProperty) as string;
            set => SetValue(ItemNameProperty, value);
        }

        [Category("UI적 요소")]
        public static DependencyProperty SourceProperty = DependencyProperty.Register(nameof(Source), typeof(ImageSource), typeof(TemplateItem));
        public ImageSource Source
        {
            get => GetValue(SourceProperty) as ImageSource;
            set => SetValue(SourceProperty, value);
        }

        public StageComponent StageComponent { get; set; }
    }
}
