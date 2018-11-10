using Delight.Core.MovingLight.Effects__.Setters.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Core.MovingLight.Effects__.Setters
{
    public class ValuesSetter : BaseSetter
    {
        public List<ValueSetter> ValueSetters { get; set; }
    }
}
