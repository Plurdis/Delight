using Delight.Component.MovingLight.Effects.Values.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Component.MovingLight.Effects.Values
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

        public static explicit operator StaticValue(byte b)  // explicit byte to digit conversion operator
        {
            StaticValue v = new StaticValue(b);

            return v;
        }
    }
}
