using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Core.Sources
{
    public class TemplateData
    {
        public string Id { get; set; }
        public string FileName { get; set; }
        public bool StreamUse { get; set; }
        public Stream Stream { get; set; }
    }
}
