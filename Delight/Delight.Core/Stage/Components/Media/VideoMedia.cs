using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Core.Stage.Components.Media
{
    public class VideoMedia : BaseMedia
    {
        public VideoMedia() : base(SourceType.Video, false)
        {
        }
    }
}
