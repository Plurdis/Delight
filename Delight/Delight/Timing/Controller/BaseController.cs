using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

using Delight.Common;
using Delight.Controls;
using Delight.Core.Common;

namespace Delight.Timing.Controller
{
    public abstract class BaseController
    {
        public BaseController(Track track, DelayTimingReader reader, bool waitWhileLoading = false)
        {
            reader.ItemEnded += Reader_ItemEnded;
            reader.ItemPlaying += Reader_ItemPlaying;
            reader.TimeLineStopped += Reader_TimeLineStopped;
            reader.ItemReady += Reader_ItemReady;
            Reader = reader;
            Track = track;

            this.waitWhileLoading = waitWhileLoading;
        }

        bool waitWhileLoading = false;

        private void Reader_ItemReady(TrackItem sender, TimingReadyEventArgs e)
        {
            ItemReady(sender, e);
            if (!waitWhileLoading)
            {
                Reader.DoneTask();
            }
        }

        internal int CurrentFrame => Reader.CurrentFrame;
        internal FrameRate CurrentFrameRate => Reader.CurrentFrameRate;
        internal DelayTimingReader Reader { get; }

        Track Track { get; }

        private void Reader_TimeLineStopped(object sender, EventArgs e)
        {
            TimeLineStopped();
        }

        private void Reader_ItemPlaying(TrackItem sender, TimingEventArgs e)
        {
            var parent = sender.Parent;
            if (parent == null)
                return;

            if ((parent as Grid).TemplatedParent is Track track)
            {
                if (this.Track == track)
                    ItemPlaying(sender, e);
            }
        }

        private void Reader_ItemEnded(TrackItem sender, TimingEventArgs e)
        {
            var parent = sender.Parent;
            if (parent == null)
                return;

            if ((parent as Grid).TemplatedParent is Track track)
            {
                if (this.Track == track)
                    ItemEnded(sender, e);
            }
            Console.WriteLine(sender.Text + " item Ended");
        }

        public abstract void ItemPlaying(TrackItem sender, TimingEventArgs e);

        public abstract void ItemEnded(TrackItem sender, TimingEventArgs e);

        public abstract void ItemReady(TrackItem sender, TimingReadyEventArgs e);

        public abstract void TimeLineStopped();
    }
}
