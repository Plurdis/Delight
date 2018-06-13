using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StagePainter.Core.Extension
{
    public static class ObjectEx
    {
        public static T GetAttribute<T>(this object obj) where T : Attribute
        {
            Type type = obj.GetType();

            return type.GetCustomAttributes(true)
                       .Where(i => i is T)
                       .FirstOrDefault() as T;
        }

        public static bool HasAttribute<T>(this object obj) where T : Attribute
        {
            return (obj.GetAttribute<T>() == null);
        }
    }
}
