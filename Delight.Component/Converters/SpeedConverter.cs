using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            string pattern = @"([\d\.]+)";

            string matchValue = Regex.Match(value, pattern).Groups[1].Value;

            if (string.IsNullOrEmpty(matchValue))
            {
                return 0;
            }
            else
            {
                double speedValue = double.Parse(matchValue);

                if (speedValue > 10)
                {
                    speedValue = 10;
                }

                return speedValue;
            }
        }
    }
}
