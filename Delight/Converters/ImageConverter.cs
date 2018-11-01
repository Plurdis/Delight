using Delight.Component.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Delight.Converters
{
    public class ImageConverter : ValueConverter<Uri, ImageSource>
    {
        public override ImageSource Convert(Uri value, Type targetType, object parameter, CultureInfo culture)
        {
            return new BitmapImage(value);
        }

        public override Uri ConvertBack(ImageSource value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is BitmapImage bi)
            {
                return bi.UriSource;
            }

            return null;
        }
    }
}
