using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Component.Converters
{
    class ByteToPercentageConverter : ValueConverter<byte, double>
    {
        public override double Convert(byte value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((double)value / byte.MaxValue);
        }

        public override byte ConvertBack(double value, Type targetType, object parameter, CultureInfo culture)
        {
            return (byte)(value * byte.MaxValue);
        }
    }
}
