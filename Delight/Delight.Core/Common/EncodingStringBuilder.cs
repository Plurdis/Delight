using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Core.Common
{
    public class EncodingStringWriter : StringWriter
    {
         public EncodingStringWriter(StringBuilder stringBuilder) : base(stringBuilder)
         {
         }

         public override Encoding Encoding => Encoding.UTF8; 
    }
}
