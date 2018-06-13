using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StagePainter.Core.IO.TextFormatter
{
    public class XMLFormatter : BaseTextFormatter
    {
        public XMLFormatter(object obj) : base(obj)
        {
        }

        public override string TextFormat()
        {
            return string.Empty;
        }

        // To-Do: Rebuild String Level Struct
    }
}
