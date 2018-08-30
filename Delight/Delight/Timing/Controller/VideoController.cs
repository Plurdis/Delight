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

        private void ItemStarted()
        {
        }
        /*
         
                Task.Run(() =>
                {
                    LoadWaitingVideos();
                    
                    player1.Dispatcher.Invoke(() =>
                    {
                        void localPositionChanged(MediaElementPro s, TimeSpan p)
                        {
                            s.PositionChanged -= localPositionChanged;

                            TrackItem itm = s.GetTag<TrackItem>();
                            s.Position = MediaTools.FrameToTimeSpan(itm.ForwardOffset + TimeLine.Position - itm.Offset, TimeLine.FrameRate);
                            s.Volume = 1;
                            s.Visibility = Visibility.Visible;
                            if (s == player1)
                                player2.Visibility = Visibility.Hidden;
                            else
                                player1.Visibility = Visibility.Hidden;
                        }   
                        MainWindow mw = Application.Current.MainWindow as MainWindow;

                        if (player1.Tag == null && player2.Tag == null)
                        {
                            return;
                        }

                        if (player1.IsPlaying && player1.Source != null)
                        {
                            var p1Tag = player1.GetTag<TrackItem>();
                            if (TimeLine.Position == p1Tag.Offset + p1Tag.FrameWidth)
                            {
                                DisablePlayer(player1);
                            }
                        }

                        if (player2.IsPlaying && player2.Source != null)
                        {
                            var p2Tag = player2.GetTag<TrackItem>();
                            if (TimeLine.Position == p2Tag.Offset + p2Tag.FrameWidth)
                            {
                                DisablePlayer(player2);
                            }
                        }

                        DebugHelper.WriteLine(player1.Position + " :: " + player2.Position);
                        if (!p1Playing)
                        {
                            if (!player1.IsPlaying && player1.Tag != null &&
                                TimeLine.Position - 1 > player1.GetTag<TrackItem>().Offset)
                            {
                                player1.Play();
                                player1.PositionChanged += localPositionChanged;
                            }
                            p1Playing = true;
                        }
                        else
                        {
                            if (!player2.IsPlaying && player2.Tag != null &&
                                TimeLine.Position - 1 > player2.GetTag<TrackItem>().Offset)
                            {
                                player2.Play();
                                player2.PositionChanged += localPositionChanged;
                            }

                            p1Playing = false;
                        }

                    });
                });
         */
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
            Console.WriteLine($"Current Frame : {e.Frame - sender.Offset}");
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
