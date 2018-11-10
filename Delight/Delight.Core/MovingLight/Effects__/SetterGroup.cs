using Delight.Core.MovingLight.Effects__.Setters;
using Delight.Core.MovingLight.Effects__.Setters.Base;
using Delight.Core.MovingLight.Effects__.Values;
using Delight.Core.MovingLight.Effects__.Values.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Delight.Core.MovingLight.Effects__
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

        public void AddStaticState(int port, byte value)
        {
            Setters.Add(new ValueSetter()
            {
                Port = port,
                Value = new StaticValue(value),
            });
        }

        public void AddPropertyState(int port, string propName)
        {
            Setters.Add(new ValueSetter()
            {
                Port = port,
                Value = new PropertyValue(propName),
            });
        }

        public void AddStates(params (int, BaseValue)[] values)
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
