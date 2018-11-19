using Delight.Core.MovingLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Component.MovingLight.Effects
{
    public class SetterProperty
    {
        public SetterProperty()
        {

        }

        public SetterProperty(PortNumber portNumber, string propertyName, string displayName, byte initializeValue, bool isStatic = false)
        {
            PortNumber = portNumber;
            PropertyName = propertyName;
            DisplayName = displayName;
            InitializeValue = initializeValue;
            IsStatic = isStatic;
        }

        public byte? InitializeValue { get; set; }

        public PortNumber PortNumber { get; set; }

        public string PropertyName { get; set; }

        public string DisplayName { get; set; }

        public bool IsStatic { get; set; }
    }
}
