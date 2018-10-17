using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Delight.Timing;

namespace Delight.Components
{
    public class MovingLight : StageComponent
    {
        public MovingLight() : base(TrackType.Light, false)
        {
        }

        public string MovingPreset { get; set; }
    }
}
