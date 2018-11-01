using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Delight.Component.Extensions
{
    public static class FrameworkElementEx
    {
        public static T GetTag<T>(this FrameworkElement obj)
        {
            return (T)obj.Tag;
        }

        public static void SetLeftMargin(this FrameworkElement element, double value)
        {
            element.Margin = element.Margin.ChangeLeft(value);
        }

        public static void SetRightMargin(this FrameworkElement element, double value)
        {
            element.Margin = element.Margin.ChangeRight(value);
        }
    }
}
