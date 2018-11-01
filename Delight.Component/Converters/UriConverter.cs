using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Component.Converters
{
    public class UriConverter : ValueConverter<string, Uri>
    {
        public override Uri Convert(string value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            return new Uri(value);
        }

        public override string ConvertBack(Uri value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.OriginalString;
        }
    }
}
