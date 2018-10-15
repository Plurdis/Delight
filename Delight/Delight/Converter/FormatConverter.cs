using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Delight.Converter
{
    public class FormatConverter : ValueConverter<string, string>
    {
        public override string Convert(string value, Type targetType, object parameter, CultureInfo culture)
        {
            string returnValue = string.Empty;

            if (parameter is double i)
            {
                returnValue = value.Replace("%v", ((int)(i * 100)).ToString());
                returnValue = returnValue.Replace("%%", "%");
            }

            return returnValue;
        }


        public override string ConvertBack(string value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
