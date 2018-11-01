using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Component.Converters
{
    public class PercentageConverter : ValueConverter<double, string>
    {
        public override string Convert(double value, Type targetType, object parameter, CultureInfo culture)
        {
            return Math.Round(value * 100, 1) + "%";
        }

        public override double ConvertBack(string value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
