using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Delight.Common;
using Delight.Components.Common;
using Delight.Controls;
using Delight.Extensions;
using Delight.Layer;
using WPFMediaKit.DirectShow.MediaPlayers;

namespace Delight.Timing.Controller
{
    public class VideoController : BaseController
    {
        public VideoController(Track track, TimingReader reader) : base(track, reader, true)
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
            Console.WriteLine(player.Source == null ? "Null" : player.Source.ToString());
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
                Reader.DoneTask();
                return;
            }
                
            MediaElementPro player = p1Playing ? player2 : player1;
            MediaElementLoader loader = p1Playing ? loader2 : loader1;
            lastLoadItem = trackItem;
            Console.WriteLine("Load Start");

            await loader.LoadVideo(trackItem);
            Console.WriteLine("Load Complete");
            Reader.DoneTask();
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

            if (trackItem != player.GetTag<TrackItem>())
                return;
            
            Console.WriteLine($"Play{player.Source.ToString()} At {player.Name}");
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
                s.Opacity = itm.ItemProperty.Opacity;
                itm.ItemProperty.OpacityChanged += (sen, e) =>
                {
                    s.Opacity = itm.ItemProperty.Opacity;
                };
                itm.ItemProperty.SizeChanged += (sen, e) =>
                {
                    Canvas rootCanvas = ((VideoLayer)s.TemplatedParent).Parent as Canvas;

                    s.Width = rootCanvas.ActualWidth * itm.ItemProperty.Size;
                    s.Height = rootCanvas.ActualHeight * itm.ItemProperty.Size;
                };
                if (s == player1)
                    player2.Visibility = Visibility.Hidden;
                else
                    player1.Visibility = Visibility.Hidden;
            }
        }

        int lastFrame;
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
                int currentFrame = e.Frame - sender.Offset + sender.ForwardOffset;
                if (lastFrame + 1 != currentFrame)
                {
                    CurrentPlayer.Position = MediaTools.FrameToTimeSpan(currentFrame, CurrentFrameRate);
                }
                
                lastFrame = e.Frame - sender.Offset + sender.ForwardOffset;
            }
        }

        public override void ItemEnded(TrackItem sender, TimingEventArgs e)
        {
            MediaElementPro player = GetPlayer(sender);
            DisablePlayer(player);
            Items.Remove(sender);

            player.Width = double.NaN;
            player.Height = double.NaN;

            sender.ItemProperty.ResetOpacityHandler();
            sender.ItemProperty.ResetSizeHandler();
            Console.WriteLine("Item Closed");
        }

        public override void TimeLineStopped()
        {
            Items.Clear();
            lastLoadItem = null;
            p1Playing = false;
            DisablePlayer(player1);
            DisablePlayer(player2);
            Console.WriteLine("!!");
        }

        public override void ItemReady(TrackItem sender, TimingReadyEventArgs e)
        {
            LoadPlayer(sender);
            //Console.WriteLine(sender.Text + " " + e.RemainFrame);
        }
    }
}
