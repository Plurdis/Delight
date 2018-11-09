using Delight.Component.Controls;
using Delight.Component.Primitives.TimingReaders;
using Delight.Core.Common;
using System;
using System.Windows.Controls;

namespace Delight.Component.Primitives.Controllers
{
    public abstract class BaseController
    {
        public BaseController(Track track, TimingReader reader, bool waitWhileLoading = false)
        {
            reader.ItemEnded += Reader_ItemEnded;
            reader.ItemPlaying += Reader_ItemPlaying;
            reader.ItemStarted += Reader_ItemStarted;
            reader.TimeLineStopped += Reader_TimeLineStopped;
            if (reader is DelayTimingReader dReader)
            {
                dReader.ItemReady += Reader_ItemReady;
            }

            Reader = reader;
            Track = track;

            this.waitWhileLoading = waitWhileLoading;
        }

        private void Reader_ItemStarted(TrackItem sender, TimingEventArgs e)
        {
            var parent = sender.Parent;
            if (parent == null)
                return;

            if ((parent as Grid).TemplatedParent is Track track)
            {
                if (this.Track == track)
                    ItemStarted(sender, e);
            }
        }

        bool waitWhileLoading = false;

        private void Reader_ItemReady(TrackItem sender, TimingReadyEventArgs e)
        {
            if (Reader is DelayTimingReader dReader)
            {
                ItemReady(sender, e);
                if (!waitWhileLoading)
                {
                    dReader.DoneTask();
                }
            }
        }

        internal int CurrentFrame => Reader.CurrentFrame;
        internal FrameRate CurrentFrameRate => Reader.CurrentFrameRate;
        internal TimingReader Reader { get; }

        internal Track Track { get; }

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

        public abstract void ItemStarted(TrackItem sender, TimingEventArgs e);

        public abstract void ItemPlaying(TrackItem sender, TimingEventArgs e);

        public abstract void ItemEnded(TrackItem sender, TimingEventArgs e);

        public abstract void ItemReady(TrackItem sender, TimingReadyEventArgs e);

        public abstract void TimeLineStopped();
    }
}
