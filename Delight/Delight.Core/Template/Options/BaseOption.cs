using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Core.Template.Options
{
    [Serializable]
    public abstract class BaseOption
    {
        public abstract string Name { get; set; }
    }
}
