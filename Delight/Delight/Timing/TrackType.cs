using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Timing
{
    public enum TrackType
    {
        Image = 1,
        Video = 2,
        Unity = 4,
        Sound = 8,
        Effect = 16,
        Template = 32,
        Light = 64,
        Unknown = int.MaxValue,
    }
}
