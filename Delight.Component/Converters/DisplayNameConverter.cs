using Delight.Component.Controls;
using Delight.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Component.Converters
{
    public class DisplayNameConverter : ValueConverter<object, string>
    {
        public override string Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var attr = value?.GetAttribute<DesignElementAttribute>();

            if (attr == null)
                return null;

            return attr.DisplayName;
        }

        public override object ConvertBack(string value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
