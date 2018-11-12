using Delight.Component.MovingLight.Effects.Values;
using Delight.Component.MovingLight.Effects.Values.Base;
using Delight.Core.MovingLight;
using System.Xml.Serialization;

namespace Delight.Component.MovingLight.Effects.Setters
{
    public class ValueSetter : BaseSetter
    {
        public PortNumber Port { get; set; }

        [XmlElement(typeof(BaseValue))]
        [XmlElement(typeof(StaticValue))]
        [XmlElement(typeof(PropertyValue))]
        public BaseValue Value { get; set; }
    }
}
