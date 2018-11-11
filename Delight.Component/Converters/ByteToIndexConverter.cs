using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Component.Converters
{
    class ByteToIndexConverter : ValueConverter<byte, int>
    {
        public override int Convert(byte value, Type targetType, object parameter, CultureInfo culture)
        {
            int i = 0;
            for (byte b = 0; b <= 120; b += 10)
            {
                if (Include(value, b, (byte)(b + 9)))
                {
                    return i;
                }
                i++;
            }

            return -1;
        }

        public bool Include(byte b, byte l, byte h)
        {
            if (b >= l && b <= h)
                return true;

            return false;
        }

        public override byte ConvertBack(int value, Type targetType, object parameter, CultureInfo culture)
        {
            return (byte)((value * 10) + 5);
        }
    }
}
