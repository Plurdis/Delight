using Delight.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight
{
    public class TimingEventArgs
    {
        public TimingEventArgs(TimeLine timeLine, int frame)
        {
            TimeLine = timeLine;
            Frame = frame;
        }
        public int Frame { get; set; }
        public TimeLine TimeLine { get; set; }
    }

    public delegate void TimingDelegate(TrackItem sender, TimingEventArgs e);
}
