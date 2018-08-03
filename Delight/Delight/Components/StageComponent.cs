using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

using Delight.TimeLineComponents;

namespace Delight.Components
{
    public abstract class StageComponent
    {
        public StageComponent(TrackType trackType)
        {
            TrackType = trackType;
        }

        public TrackType TrackType { get; } = TrackType.Unknown;

        
        public TimeSpan Time { get; set; }

        public string Identifier { get; set; }
        
        public ImageSource Thumbnail { get; set; }
    }
}
