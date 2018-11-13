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
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
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
            player.PositionChanged += localPositionChanged;
            p1Playing = !p1Playing;
            void localPositionChanged(MediaElementPro s, TimeSpan p)
            {
                s.PositionChanged -= localPositionChanged;

                Canvas rootCanvas = ((VideoLayer)s.TemplatedParent).Parent as Canvas;
                VideoLayer rootLayer = (VideoLayer)s.TemplatedParent;

                TrackItem itm = s.GetTag<TrackItem>();
                s.Position = MediaTools.FrameToTimeSpan(itm.ForwardOffset + CurrentFrame - itm.Offset, CurrentFrameRate);
                s.Visibility = Visibility.Visible;
                //s.Volume = itm.ItemProperty.Volume;
                //s.Opacity = itm.ItemProperty.Opacity;

                s.Width = rootCanvas.ActualWidth;// * itm.ItemProperty.Size;
                s.Height = rootCanvas.ActualHeight;

                itm.PropertyChanged += (sen, e) =>
                {
                    object value = PropertyManager.GetProperty(itm.Property, e.PropertyName);

                    switch (e.PropertyName.ToLower())
                    {
                        case "left":
                            Canvas.SetLeft(rootLayer, 
                                (rootCanvas.ActualWidth - s.Width + (rootCanvas.ActualWidth * 2 * (double)value)) / 2);
                            break;
                        case "top":
                            Canvas.SetTop(rootLayer, 
                                (rootCanvas.ActualHeight - s.Height + (rootCanvas.ActualHeight * 2 * (double)value)) / 2);
                            break;
                        case "opacity":
                            s.Opacity = (double)value;
                            break;
                        case "volume":
                            s.Volume = (double)value;
                            if (s.Volume != 0)
                            {
                                ((VideoItemProperty)itm.Property).IsMute = false;
                            }
                            break;
                        case "chromakeyuse":
                        case "chromakeyusage":
                        case "chromakeycolor":
                            VideoLayer vLayer = (VideoLayer)s.TemplatedParent;
                            
                            Brush newColor = (Brush)PropertyManager.GetProperty(itm.Property, "ChromaKeyColor");
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
                                Smooth = (float)((double)PropertyManager.GetProperty(itm.Property, "ChromaKeyUsage")),
                            };

                            if ((bool)PropertyManager.GetProperty(itm.Property, "ChromaKeyUse"))
                            {
                                vLayer.Effect = _chromaKeyEffect;
                            }
                            else
                            {
                                vLayer.Effect = _chromaKeyEffect;
                            }

                            break;
                        case "ismute":
                            bool isMute = (bool)value;

                            if (isMute)
                            {
                                s.Volume = 0;
                            }
                            else
                            {
                                s.Volume = (double)PropertyManager.GetProperty(itm.Property, "Volume");
                            }
                            break;
                        default:
                            break;
                    }

                    //        case "chromakeyusage":
                    //        case "chromakeycolor":
                    //        case "chromakeyenabled":
                    //            VideoLayer vLayer = (VideoLayer)s.TemplatedParent;

                    //            if (itm.ItemProperty.ChromaKeyEnabled)
                    //            {
                    //                itm.ItemProperty.ChromaKeyColor.ToHSL(out double _h, out double _s, out double _l);

                    //                vLayer.Effect = new ChromaKeyEffect()
                    //                {
                    //                    HueMin = (float)_h,
                    //                    HueMax = (float)_h,
                    //                    SaturationMax = (float)_s,
                    //                    SaturationMin = (float)_s,
                    //                    LuminanceMax = (float)_l,
                    //                    LuminanceMin = (float)_l,
                    //                    Smooth = (float)itm.ItemProperty.ChromaKeyUsage,
                    //                };
                    //            }
                    //            else
                    //            {
                    //                vLayer.Effect = null;
                    //            }
                    //            break;
                    //    }

                };
                // * itm.ItemProperty.Size;
                //Canvas.SetLeft(rootLayer, (rootCanvas.ActualWidth - s.Width + (rootCanvas.ActualWidth * 2 * itm.ItemProperty.PositionX)) / 2);
                //Canvas.SetTop(rootLayer, (rootCanvas.ActualHeight - s.Height + (rootCanvas.ActualHeight * 2 * itm.ItemProperty.PositionY)) / 2);

                //itm.Property.PropertyChanged += (sen, e) =>
                //{
                //    string propName = e.PropertyName;

                //    void Connect()
                //    {

                //    }
                //};

                

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
