using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StagePainter.Tracks
{
    public interface ITrackItemInfo : ICloneable
    {
        double Offset { get; set; }

        double Size { get; }

        string Name { get; set; }
    }
}
