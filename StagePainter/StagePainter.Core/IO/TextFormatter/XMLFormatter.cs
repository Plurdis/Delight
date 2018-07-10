using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StagePainter.Core.IO.TextFormatter
{
    /// <summary>
    /// XML로 포맷하는 포매터입니다.
    /// </summary>
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
