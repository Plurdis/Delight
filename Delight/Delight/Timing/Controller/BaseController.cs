using Delight.Common;
using Delight.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Timing.Controller
{
    public abstract class BaseController
    {
        public BaseController(TimingReader reader)
        {
            reader.ItemEnded += Reader_ItemEnded;
            reader.ItemPlaying += Reader_ItemPlaying;
            reader.TimeLineStopped += Reader_TimeLineStopped;
        }

        private void Reader_TimeLineStopped(object sender, EventArgs e)
        {
            TimeLineStopped();
        }

        private void Reader_ItemPlaying(TrackItem sender, TimingEventArgs e)
        {
            ItemPlaying(sender, e);
        }

        private void Reader_ItemEnded(TrackItem sender, TimingEventArgs e)
        {
            Console.WriteLine(sender.Text + " item Ended");
            ItemEnded(sender, e);
        }

        public abstract void ItemPlaying(TrackItem sender, TimingEventArgs e);

        public abstract void ItemEnded(TrackItem sender, TimingEventArgs e);

        public abstract void TimeLineStopped();
    }
}
