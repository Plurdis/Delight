using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Component.Converters
{
    public class BoolToStringConverter : ValueConverter<bool, string>
    {
        public override bool ConvertBack(string value, Type targetType, object parameter, CultureInfo culture)
        {
            return bool.Parse(value);
        }

        public override string Convert(bool value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }
    }
}
