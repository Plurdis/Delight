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
    public class ItemToolTip : ToolTip
    {
        public ItemToolTip()
        {
            
        }
        public static DependencyProperty ImageProperty = DependencyProperty.Register(nameof(Image), typeof(ImageSource), typeof(ItemToolTip));

        public ImageSource Image
        {
            get => (ImageSource)GetValue(ImageProperty);
            set => SetValue(ImageProperty, value);
        }

        public static DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(ItemToolTip));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

    }
}
