using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Core.Sources.Options
{
    [Serializable]
    public class YoutubeOption : BaseOption
    {
        public override string Name { get; set; }
        public override string Tag { get; set; }
    }
}
