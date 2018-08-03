using Delight.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.TimeLineComponents
{
    public class TimeLineReader
    {
        public TimeLineReader(TimeLine timeLine)
        {
            _timeLine = timeLine;

            _timeLine.FrameChanged += _timeLine_FrameChanged;
        }

        private void _timeLine_FrameChanged(object sender, EventArgs e)
        {
            //_timeLine.Items
            Console.WriteLine("Frame Change Detected - " + _timeLine.Position);
        }

        TimeLine _timeLine;
    }
}
