using Delight.Core.MovingLight.Effects__.Values.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Core.MovingLight.Effects__.Values
{
    public class StaticValue : BaseValue
    {
        public StaticValue()
        {

        }

        public StaticValue(byte value)
        {
            Value = value;
        }
        public byte Value { get; set; }
    }
}
