using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Component.Converters
{
    public class StringToIntConverter : ValueConverter<string, int>
    {
        public override int Convert(string value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value))
                return -1;
            return int.Parse(value);
        }

        public override string ConvertBack(int value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
