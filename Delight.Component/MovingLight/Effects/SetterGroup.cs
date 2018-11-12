using Delight.Component.MovingLight.Effects.Setters;
using Delight.Component.MovingLight.Effects.Values;
using Delight.Component.MovingLight.Effects.Values.Base;
using Delight.Core.MovingLight;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Delight.Component.MovingLight.Effects
{
    public class SetterGroup
    {
        [XmlElement(typeof(BaseSetter))]
        [XmlElement(typeof(ValueSetter))]
        [XmlElement(typeof(WaitSetter))]
        [XmlElement(typeof(ContinueSetter))]
        [XmlElement(typeof(ValuesSetter))]
        public List<BaseSetter> Setters { get; set; }

        public SetterGroup()
        {
            Setters = new List<BaseSetter>();
        }

        public void AddStaticState(PortNumber port, byte value)
        {
            Setters.Add(new ValueSetter()
            {
                Port = port,
                Value = new StaticValue(value),
            });
        }

        public void AddPropertyState(PortNumber port, string propName)
        {
            Setters.Add(new ValueSetter()
            {
                Port = port,
                Value = new PropertyValue(propName),
            });
        }

        public void AddStates(params (PortNumber, BaseValue)[] values)
        {
            Setters.Add(new ValuesSetter()
            {
                ValueSetters = new List<ValueSetter>(
                    values.Select(i => new ValueSetter()
                    {
                        Port = i.Item1,
                        Value = i.Item2,
                    }))
            });
        }

        public void AddWait(int waitMilliseconds)
        {
            Setters.Add(new WaitSetter()
            {
                WaitMilliseconds = waitMilliseconds
            });
        }

        public void AddContinueLine(int milliseconds)
        {
            Setters.Add(new ContinueSetter()
            {
                ContinueMilliseconds = milliseconds
            });
        }
    }
}
