using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Components.Media
{
    public class Sound : IMedia
    {
        public string Identity { get; set; }
        
        public TimeSpan Duration { get; set; }

        public string FileLocation { get; set; }

    }
}
