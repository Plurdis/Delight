using Delight.Controls;
using Delight.Timing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight
{
    public class TrackEventArgs : EventArgs
    {
        public TrackEventArgs(Track track, TrackType trackType)
        {
            this.Track = track;
            this.TrackType = trackType;
        }

        public Track Track { get; }

        public TrackType TrackType { get; }
    }
}
