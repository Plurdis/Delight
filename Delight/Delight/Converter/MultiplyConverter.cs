using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Delight.Converter
{
    class MultiplyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int param = (int)parameter;

            return (int)((double)value * param);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int param = (int)parameter;

            return (int)((double)value / param);
        }
    }
}
