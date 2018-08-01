using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Delight.Extensions
{
    public static class ThicknessEx
    {
        public static Thickness ChangeLeft(this Thickness thickness, double value)
        {
            return new Thickness(value, thickness.Top, thickness.Right, thickness.Bottom);
        }
        public static Thickness ChangeRight(this Thickness thickness, double value)
        {
            return new Thickness(thickness.Left, thickness.Top, value, thickness.Bottom);
        }
    }
}
