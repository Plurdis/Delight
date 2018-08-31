using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Core.Extensions
{ 
    public static class IEnumerableEx
    {
        public static void ForEach<T>(this IEnumerable<T> ienumerable, Action<T> action)
        {
            ienumerable.ToList().ForEach(action);
        }
    }
}
