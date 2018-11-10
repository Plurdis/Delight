using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Core.MovingLight.Effects__
{
    public class SetterProperty
    {
        public SetterProperty()
        {

        }

        public SetterProperty(PortNumber portNumber, string propertyName)
        {
            PortNumber = portNumber;
            PropertyName = propertyName;
        }
        public PortNumber PortNumber { get; set; }

        public string PropertyName { get; set; }
    }
}
