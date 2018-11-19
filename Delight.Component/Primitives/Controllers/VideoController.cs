using Delight.Component.Common;
using Delight.Component.Controls;
using Delight.Component.Effects;
using Delight.Component.Extensions;
using Delight.Component.ItemProperties;
using Delight.Component.Layers;
using Delight.Component.Primitives.TimingReaders;
using Delight.Core.Common;
using Delight.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Threading;
using WPFMediaKit.DirectShow.MediaPlayers;

namespace Delight.Component.Primitives.Controllers
{
    public class VideoController : BaseController
    {

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
                ((DelayTimingReader)Reader).DoneTask();
                return;
            }

            MediaElementPro player = p1Playing ? player2 : player1;
            MediaElementLoader loader = p1Playing ? loader2 : loader1;
            lastLoadItem = trackItem;

            await loader.LoadVideo(trackItem);
            ((DelayTimingReader)Reader).DoneTask();
        }

        public VideoController(Track track, DelayTimingReader reader) : base(track, reader, true)
        {
        }

        Effect _chromaKeyEffect;

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

            Canvas rootCanvas = ((VideoLayer)player.TemplatedParent).Parent as Canvas;
            VideoLayer rootLayer = (VideoLayer)player.TemplatedParent;

            player.PositionChanged += localPositionChanged;
            
            p1Playing = !p1Playing;


            trackItem.PropertyChanged += (sen, e) =>
            {
                object value = PropertyManager.GetProperty(trackItem.Property, e.PropertyName);

                switch (e.PropertyName.ToLower())
                {
                    case "left":
                    case "top":
                    case "scale":
                        rootLayer.Width = (double.IsNaN(rootCanvas.Width) ? rootCanvas.ActualWidth : rootCanvas.Width) * (double)PropertyManager.GetProperty(trackItem.Property, "Scale");
                        rootLayer.Height = (double.IsNaN(rootCanvas.Height) ? rootCanvas.ActualWidth : rootCanvas.Height) * (double)PropertyManager.GetProperty(trackItem.Property, "Scale");
                        player.Width = rootLayer.Width;
                        player.Height = rootLayer.Height;

                        Canvas.SetLeft(rootLayer,
                            (rootCanvas.ActualWidth - player.Width + (rootCanvas.ActualWidth * 2 * (double)PropertyManager.GetProperty(trackItem.Property, "Left"))) / 2);

                        Canvas.SetTop(rootLayer,
                            (rootCanvas.ActualHeight - player.Height + (rootCanvas.ActualHeight * 2 * (double)PropertyManager.GetProperty(trackItem.Property, "Top"))) / 2);
                        break;
                    case "opacity":
                        player.Opacity = (double)value;
                        break;
                    case "volume":

                        var volume = (((double)value) * 0.5);
                        Console.WriteLine(trackItem.Text + " : " + volume);
                        if (volume == 0)
                        {
                            Application.Current.MainWindow.Dispatcher.Invoke(() =>
                            {
                                player.Volume = 0;
                            });
                        }

                        else
                            player.Volume = volume + 0.5;

                        if ((bool)PropertyManager.GetProperty(trackItem.Property, "IsMute") && player.Volume != 0)
                        {
                            ((VideoItemProperty)trackItem.Property).IsMute = false;
                        }
                        break;
                    case "chromakeyuse":
                    case "chromakeyusage":
                    case "chromakeycolor":
                        VideoLayer vLayer = (VideoLayer)player.TemplatedParent;

                        Brush newColor = (Brush)PropertyManager.GetProperty(trackItem.Property, "ChromaKeyColor");
                        SolidColorBrush newBrush = (SolidColorBrush)newColor;
                        newBrush.Color.ToHSL(out double _h, out double _s, out double _l);
                        _chromaKeyEffect = new ChromaKeyEffect()
                        {
                            HueMin = (float)_h,
                            HueMax = (float)_h,
                            SaturationMax = (float)_s,
                            SaturationMin = (float)_s,
                            LuminanceMax = (float)_l,
                            LuminanceMin = (float)_l,
                            Smooth = (float)((double)PropertyManager.GetProperty(trackItem.Property, "ChromaKeyUsage")),
                        };

                        if ((bool)PropertyManager.GetProperty(trackItem.Property, "ChromaKeyUse"))
                            vLayer.Effect = _chromaKeyEffect;
                        else
                            vLayer.Effect = null;

                        break;
                    case "ismute":
                        bool isMute = (bool)value;

                        if (isMute)
                            player.Volume = 0;
                        else
                            player.Volume = (double)PropertyManager.GetProperty(trackItem.Property, "Volume");
                        break;
                    default:
                        break;
                }
            };

            foreach (PropertyInfo allProperties in trackItem.Property.GetType().GetRuntimeProperties())
            {
                trackItem.OnPropertyChanged(allProperties.Name);
            }

            void localPositionChanged(MediaElementPro s, TimeSpan p)
            {
                s.PositionChanged -= localPositionChanged;
                
                TrackItem itm = s.GetTag<TrackItem>();
                s.Position = MediaTools.FrameToTimeSpan(itm.ForwardOffset + CurrentFrame - itm.Offset, CurrentFrameRate);
                s.Visibility = Visibility.Visible;

                s.Width = rootCanvas.ActualWidth;// * itm.ItemProperty.Size;
                s.Height = rootCanvas.ActualHeight;
                
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
            if (player != null)
            {
                player.Width = double.NaN;
                player.Height = double.NaN;

                VideoLayer rootLayer = (VideoLayer)player.TemplatedParent;

                Canvas.SetLeft(rootLayer, 0);
                Canvas.SetTop(rootLayer, 0);
            }

            //sender.ItemProperty.ResetEventHandler();
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
            //Console.WriteLine(sender.Text + " " + e.RemainFrame);
        }

        public override void ItemStarted(TrackItem sender, TimingEventArgs e)
        {
        }
    }
}
