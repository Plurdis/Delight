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
            try
            {
                return ((double)value / byte.MaxValue);
            }
            catch (Exception)
            {
                return 0;
            }
            
        }

        public override byte ConvertBack(double value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return (byte)(value * byte.MaxValue);
            }
            catch (Exception)
            {
                return 0;
            }
            
        }
    }
}
