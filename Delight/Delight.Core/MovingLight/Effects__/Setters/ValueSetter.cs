using Delight.Core.MovingLight.Effects;
using Delight.Core.MovingLight.Effects__.Setters.Base;
using Delight.Core.MovingLight.Effects__.Values;
using Delight.Core.MovingLight.Effects__.Values.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Delight.Core.MovingLight.Effects__.Setters
{
    public class ValueSetter : BaseSetter
    {
        public int Port { get; set; }

        [XmlElement(typeof(BaseValue))]
        [XmlElement(typeof(StaticValue))]
        [XmlElement(typeof(PropertyValue))]
        public BaseValue Value { get; set; }
    }
}
