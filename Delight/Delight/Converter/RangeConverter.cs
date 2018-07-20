using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Delight.Converter
{
    class RangeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (parameter is string param)
                {
                    double iValue = (double)value;
                    string fParam = param.Split('-')[0];
                    string sParam = param.Split('-')[1];

                    int lRange, hRange;
                    lRange = string.IsNullOrWhiteSpace(fParam) ? 0 : int.Parse(fParam);
                    hRange = string.IsNullOrWhiteSpace(sParam) ? int.MaxValue : int.Parse(sParam);
                    
                    if (iValue >= lRange && iValue <= hRange)
                        return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
