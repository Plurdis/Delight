using Delight.Core.MovingLight.Effects__.Values.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Core.MovingLight.Effects__.Values
{
    public class PropertyValue : BaseValue
    {
        public PropertyValue()
        {

        }
        public PropertyValue(string propertyName)
        {
            PropertyName = propertyName;
        }
        public string PropertyName { get; set; }
    }
}
