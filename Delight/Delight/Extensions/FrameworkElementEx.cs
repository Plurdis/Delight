using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Delight.Extensions
{
    public static class FrameworkElementEx
    {
        public static T GetTag<T>(this FrameworkElement obj)
        {
            return (T)obj.Tag;
        }
    }
}
