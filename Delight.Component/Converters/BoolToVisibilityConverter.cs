using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Delight.Component.Converters
{
    public class BoolToVisibilityConverter : ValueConverter<bool, Visibility>
    {
        public Visibility TrueValue { get; set; } = Visibility.Visible;
        public Visibility FalseValue { get; set; } = Visibility.Collapsed;

        public override Visibility Convert(bool value, Type targetType, object parameter, CultureInfo culture)
        {
            return value ? Visibility.Visible : Visibility.Hidden;
        }

        public override bool ConvertBack(Visibility value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == Visibility.Visible;
        }
    }
}
