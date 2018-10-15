using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Delight.Converter
{
    public abstract class ValueConverter<TFrom, TTo> : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert((TFrom)value, targetType, parameter, culture);
        }

        public abstract TTo Convert(TFrom value, Type targetType, object parameter, CultureInfo culture);

        public abstract TFrom ConvertBack(TTo value, Type targetType, object parameter, CultureInfo culture);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ConvertBack((TTo)value, targetType, parameter, culture);
        }
    }
}
