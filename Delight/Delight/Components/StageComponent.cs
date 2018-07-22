using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Components
{
    public abstract class StageComponent
    {
        public TimeSpan Time { get; set; }

        public string Identifier { get; set; }
    }
}
