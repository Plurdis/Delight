using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Delight.TimeLineComponents;

namespace Delight.Components.Medias
{
    public abstract class Media : StageComponent
    {
        public Media(TrackType trackType) : base(trackType)
        {
        }

        public string OriginalPath { get; set; }
    }
}
