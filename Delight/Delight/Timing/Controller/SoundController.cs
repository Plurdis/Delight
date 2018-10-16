using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Delight.Components.Common;
using Delight.Controls;

namespace Delight.Timing.Controller
{
    public class SoundController : BaseController
    {
        public SoundController(Track track, 
            TimingReader reader, 
            bool waitWhileLoading = false) : base(track, reader, waitWhileLoading)
        {
        }

        private MediaPlayer soundPlayer = new MediaPlayer();

        public override void ItemEnded(TrackItem sender, TimingEventArgs e)
        {
            soundPlayer.Stop();
        }

        public override void ItemPlaying(TrackItem sender, TimingEventArgs e)
        {
            
        }

        public override void ItemReady(TrackItem sender, TimingReadyEventArgs e)
        {
            
        }

        public override void ItemStarted(TrackItem sender, TimingEventArgs e)
        {
            soundPlayer.Open(new Uri(sender.OriginalPath));
            soundPlayer.Play();
            soundPlayer.Position = MediaTools.FrameToTimeSpan(e.Frame, e.TimeLine.FrameRate);
            //Console.WriteLine($"At {e.Frame} Sound Started!!!!!!!!!!!!!!!");
        }

        public override void TimeLineStopped()
        {
            
        }
    }
}
