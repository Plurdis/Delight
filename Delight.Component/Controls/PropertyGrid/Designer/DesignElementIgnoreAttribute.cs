using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Component.Controls
{
    public class DesignElementIgnoreAttribute : Attribute
    {
        public string[] PropertyNames { get; set; }

        public DesignElementIgnoreAttribute(params string[] propertyNames)
        {
            this.PropertyNames = propertyNames;
        }
    }
}
