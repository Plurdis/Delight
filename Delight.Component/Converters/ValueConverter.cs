using Delight.Component.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Delight.Component.Converters
{
    public abstract class ValueConverter<TFrom, TTo> : MarkupSupport, IValueConverter
    {
        public abstract TTo Convert(TFrom value, Type targetType, object parameter, CultureInfo culture);

        public abstract TFrom ConvertBack(TTo value, Type targetType, object parameter, CultureInfo culture);

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert((TFrom)value, targetType, parameter, culture);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ConvertBack((TTo)value, targetType, parameter, culture);
        }
    }
}
