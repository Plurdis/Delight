using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StagePainter.Core.IO.TextFormatter
{
    public abstract class BaseTextFormatter
    {
        public BaseTextFormatter(object obj)
        {
            this.Object = obj;
        }

        public object Object { get; set; }

        public abstract string TextFormat();
    }
}
