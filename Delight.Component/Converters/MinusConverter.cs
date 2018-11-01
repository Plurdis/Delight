using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Delight.Component.Converters
{
    class MinusConverter : ValueConverter<double, double>
    {
        public override double Convert(double value, Type targetType, object parameter, CultureInfo culture)
        {
            return value - (double)parameter;
        }

        public override double ConvertBack(double value, Type targetType, object parameter, CultureInfo culture)
        {
            return value + (double)parameter;
        }
    }
}
