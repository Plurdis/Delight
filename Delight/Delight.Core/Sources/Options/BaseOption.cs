using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Core.Sources.Options
{
    [Serializable]
    public abstract class BaseOption
    {
        public abstract string Name { get; set; }

        public abstract string Tag { get; set; }
    }
}
