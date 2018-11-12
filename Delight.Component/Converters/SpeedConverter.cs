using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Component.Converters
{
    class SpeedConverter : ValueConverter<double, string>
    {
        public override string Convert(double value, Type targetType, object parameter, CultureInfo culture)
        {
            return Math.Round(value, 2) + "배";
        }

        public override double ConvertBack(string value, Type targetType, object parameter, CultureInfo culture)
        {
            return 0;
        }
    }
}
