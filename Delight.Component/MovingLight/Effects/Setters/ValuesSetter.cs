using Delight.Component.MovingLight.Effects.Setters.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Component.MovingLight.Effects.Setters
{
    public class ValuesSetter : BaseSetter
    {
        public List<ValueSetter> ValueSetters { get; set; }
    }
}
