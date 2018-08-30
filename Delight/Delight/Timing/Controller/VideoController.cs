using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Delight.Common;
using Delight.Controls;

using WPFMediaKit.DirectShow.MediaPlayers;

namespace Delight.Timing.Controller
{
    public class VideoController : BaseController
    {
        public VideoController(Track track, TimingReader reader) : base(track, reader)
        {
        }

        
        MediaElementPro player1, player2;
        MediaElementLoader loader1, loader2;

        List<TrackItem> Items = new List<TrackItem>();

        public void SetPlayer(MediaElementPro player1, MediaElementPro player2)
        {
            if (player1 == player2)
                throw new Exception("player1와 player2는 같은 인스턴스 일 수 없습니다.");

            this.player1 = player1;
            this.player2 = player2;

            player1.VideoRenderer = VideoRendererType.VideoMixingRenderer9;
            player2.VideoRenderer = VideoRendererType.VideoMixingRenderer9;

            player1.CurrentStateChanged += Player_CurrentStateChanged;
            player2.CurrentStateChanged += Player_CurrentStateChanged;

            loader1 = new MediaElementLoader(player1);
            loader2 = new MediaElementLoader(player2);
        }

        public void DisablePlayer(MediaElementPro player)
        {
            player.Stop();
            player.Close();
            player.Source = null;
            player.Volume = 0;
            player.Play();
            player.Visibility = Visibility.Hidden;
        }

        private void Player_CurrentStateChanged(MediaElementPro sender, PlayerState state)
        {
            if (state == PlayerState.Ended)
            {
                sender.Close();
            }
        }

        public override void ItemPlaying(TrackItem sender, TimingEventArgs e)
        {
            if (!Items.Contains(sender))
            {
                Items.Add(sender);
                Console.WriteLine(sender.Text + " item Playing");
            }
            int currentFrame = e.Frame - sender.Offset;


        }

        public override void ItemEnded(TrackItem sender, TimingEventArgs e)
        {
            
        }

        public override void TimeLineStopped()
        {
            Items.Clear();
        }
    }
}
