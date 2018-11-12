using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Delight.Component.Converters
{
    public class ByteToPercentageStringConverter : ValueConverter<byte, string>
    {
        public override string Convert(byte value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return Math.Round((decimal)((double)value / 255 * 100), 1) + "%";
            }
            catch (Exception)
            {
                return "0%";
            }
        }

        public override byte ConvertBack(string value, Type targetType, object parameter, CultureInfo culture)
        {
            string regexValue = Regex.Match(value, @"\d+").Groups[0].Value;
            int intValue = int.Parse(regexValue);

            return (byte)((double)(255 * intValue) / 100);
        }
    }
}
