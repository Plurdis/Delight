using Delight.Component.Extensions;
using System;
using System.Globalization;

namespace Delight.Component.Converters
{
    class DoubleToStringConverter : ValueConverter<double, string>
    {
        public override string Convert(double value, Type targetType, object parameter, CultureInfo culture)
        {
            if (double.IsNaN(value))
                return "자동";

            return value.ToString();
        }

        public override double ConvertBack(string value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToDouble();
        }
    }
}
