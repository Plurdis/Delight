using Delight.Component.MovingLight.Effects.Values;
using Delight.Component.MovingLight.Effects.Values.Base;
using Delight.Core.MovingLight;
using System.Xml.Serialization;

namespace Delight.Component.MovingLight.Effects.Setters
{
    public class ValueSetter : BaseSetter
    {
        public ValueSetter()
        {

        }

        public ValueSetter(PortNumber port, BaseValue value)
        {
            Port = port;
            Value = value;
        }

        public ValueSetter(PortNumber port, byte staticValue) : this(port, new StaticValue(staticValue))
        {
        }

        public ValueSetter(PortNumber port, string propertyValue) : this(port, new PropertyValue(propertyValue))
        {
        }

        public PortNumber Port { get; set; }

        [XmlElement(typeof(BaseValue))]
        [XmlElement(typeof(StaticValue))]
        [XmlElement(typeof(PropertyValue))]
        public BaseValue Value { get; set; }
    }
}
