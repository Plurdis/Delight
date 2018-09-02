using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using Delight.Common;
using Delight.Components.Common;
using Delight.Controls;
using Delight.Extensions;
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

        bool p1Playing = false;

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

        public MediaElementPro GetPlayer(TrackItem item)
        {
            if (player1.GetTag<TrackItem>() == item)
                return player1;
            else if (player2.GetTag<TrackItem>() == item)
                return player2;
            return null;
        }

        public MediaElementPro CurrentPlayer => p1Playing ? player1 : player2;

        public void DisablePlayer(TrackItem item)
        {
            DisablePlayer(GetPlayer(item));
        }

        public void DisablePlayer(MediaElementPro player)
        {
            if (player == null)
                return;

            player.Stop();
            player.Close();
            player.Source = null;
            player.Tag = null;
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

        TrackItem lastLoadItem;

        public async void LoadPlayer(TrackItem trackItem)
        {
            if (lastLoadItem == trackItem)
            {
                Console.WriteLine("lastLoadItem and TrackItem is Same!");
                return;
            }
                
            MediaElementPro player = p1Playing ? player2 : player1;
            MediaElementLoader loader = p1Playing ? loader2 : loader1;
            lastLoadItem = trackItem;
            Console.WriteLine("Load Start");

            await loader.LoadVideo(trackItem);
            Console.WriteLine("Load Complete");
            Reader.DoneTask();
            //void PlayVideo()
            //{
            //    Thread thr = new Thread(() =>
            //    {
            //        while (true)
            //        {
            //            player.Dispatcher.Invoke(() =>
            //            {
            //                var itm = player.GetTag<TrackItem>();
            //            });
            //            Thread.Sleep(10);
            //        }
            //    });
            //}

            //PlayVideo();
        }

        public async void PlayPlayer(TrackItem trackItem)
        {
            MediaElementPro player = p1Playing ? player2 : player1;
            MediaElementLoader loader = p1Playing ? loader2 : loader1;

            if (!player.VideoLoaded)
            {
                if (player.GetTag<TrackItem>() == null)
                    player.Tag = trackItem;
                
                await loader.LoadVideo(player.GetTag<TrackItem>());
            }

            player.Play();
            player.PositionChanged += localPositionChanged;
            p1Playing = !p1Playing;
            void localPositionChanged(MediaElementPro s, TimeSpan p)
            {
                s.PositionChanged -= localPositionChanged;

                TrackItem itm = s.GetTag<TrackItem>();
                s.Position = MediaTools.FrameToTimeSpan(itm.ForwardOffset + CurrentFrame - itm.Offset, CurrentFrameRate);
                s.Volume = 1;
                s.Visibility = Visibility.Visible;
                if (s == player1)
                    player2.Visibility = Visibility.Hidden;
                else
                    player1.Visibility = Visibility.Hidden;
            }
        }

        public override void ItemPlaying(TrackItem sender, TimingEventArgs e)
        {
            if (!Items.Contains(sender))
            {
                Items.Add(sender);
                PlayPlayer(sender);
                Console.WriteLine($"{sender.Text} item Playing");
            }

            if (sender == CurrentPlayer.GetTag<TrackItem>())
            {
                //Console.WriteLine($"{sender.Text}'s Current Frame : {e.Frame - sender.Offset + sender.ForwardOffset}");
            }
            int currentFrame = e.Frame - sender.Offset;
        }

        public override void ItemEnded(TrackItem sender, TimingEventArgs e)
        {
            DisablePlayer(GetPlayer(sender));
            Items.Remove(sender);
            Console.WriteLine("Item Closed");
        }

        public override void TimeLineStopped()
        {
            Items.Clear();
            lastLoadItem = null;
            p1Playing = false;
            DisablePlayer(player1);
            DisablePlayer(player2);
        }

        public override void ItemReady(TrackItem sender, TimingReadyEventArgs e)
        {
            LoadPlayer(sender);
            Console.WriteLine(sender.Text + " " + e.RemainFrame);
        }
    }
}
