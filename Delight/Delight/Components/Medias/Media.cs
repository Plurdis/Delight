using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Delight.Timing;

namespace Delight.Components.Medias
{
    public abstract class Media : StageComponent
    {
        public Media(TrackType trackType, bool maxSizeFixed = true) : base(trackType, maxSizeFixed)
        {
        }

        public string OriginalPath { get; set; }
    }
}
