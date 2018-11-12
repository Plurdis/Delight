using Delight.Component.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Delight.Component.Controls
{
    public static class Hint
    {
        public static readonly DependencyProperty TextProperty =
            DependencyHelper.RegisterAttached<string>();

        public static string GetText(this DependencyObject obj)
        {
            return obj.GetValue<string>(TextProperty);
        }

        public static void SetText(this DependencyObject obj, string value)
        {
            obj.SetValue(TextProperty, value);
        }
    }
}
