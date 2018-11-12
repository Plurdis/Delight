using System.Windows;

namespace Delight.Component.Extensions
{
    public static class DependencyObjectEx
    {
        public static T GetValue<T>(this DependencyObject obj, DependencyProperty property)
        {
            return (T)obj.GetValue(property);
        }
    }
}
