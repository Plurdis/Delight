using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Core.Attributes
{
    public class IsVisualTrackAttribute : Attribute
    {
        public IsVisualTrackAttribute(bool isVisualTrack)
        {
            IsVisualTrack = isVisualTrack;
        }

        public bool IsVisualTrack { get; }
    }
}
