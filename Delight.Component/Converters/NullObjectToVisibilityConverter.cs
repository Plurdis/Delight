using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Delight.Component.Converters
{
    public class NullObjectToVisibilityConverter : ValueConverter<object, Visibility>
    {
        public Visibility NullValue { get; set; } = Visibility.Collapsed;

        public Visibility NotNullValue { get; set; } = Visibility.Visible;

        public override Visibility Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value == null ? NullValue : NotNullValue);
        }

        public override object ConvertBack(Visibility value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
