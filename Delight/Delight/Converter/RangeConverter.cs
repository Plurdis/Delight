using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Delight.Converter
{
    class RangeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is string param)
            {
                double iValue = (double)value;
                string pattern = @"(\d+)-(\d+)";

                Match m = Regex.Match(param, pattern);

                if (m.Success)
                {
                    string fParam = m.Groups[1].Value;
                    string sParam = m.Groups[2].Value;
                    int lRange, hRange;
                    lRange = string.IsNullOrWhiteSpace(fParam) ? 0 : int.Parse(fParam);
                    hRange = string.IsNullOrWhiteSpace(sParam) ? int.MaxValue : int.Parse(sParam);

                    if (iValue >= lRange && iValue <= hRange)
                        return true;
                }
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
