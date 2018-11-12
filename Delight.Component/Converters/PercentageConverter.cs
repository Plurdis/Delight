using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Delight.Component.Converters
{
    public class PercentageConverter : ValueConverter<double, string>
    {

        public double Maximum { get; set; } = 1;
        public double Minimum { get; set; } = 0;

        public override string Convert(double value, Type targetType, object parameter, CultureInfo culture)
        {
            return $"{Math.Round((value - Minimum) / (Maximum - Minimum) * 100d, 0)}%";
        }

        public override double ConvertBack(string value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!Regex.IsMatch(value, @"\d+"))
                throw new Exception();

            int v = int.Parse(Regex.Match(value, @"\d+").Value);

            v = Math.Max(v, 0);
            v = Math.Min(v, 100);

            return v / 100d * (Maximum - Minimum) + Minimum;
        }
    }
}
