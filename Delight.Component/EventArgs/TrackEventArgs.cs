using Delight.Component.Controls;
using Delight.Core.Stage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Component
{
    public class TrackEventArgs : EventArgs
    {
        public TrackEventArgs(Track track, SourceType sourceType)
        {
            this.Track = track;
            this.SourceType = sourceType;
        }

        public Track Track { get; }

        public SourceType SourceType { get; }
    }
}
