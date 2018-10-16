using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Delight.Controls;
using Delight.Effects;
using Delight.Extensions;
using Delight.Layer;

namespace Delight.Timing.Controller
{
    public class ImageController : BaseController
    {
        public ImageController(Track track, TimingReader reader) : base(track, reader, false)
        {
        }

        public ImageLayer Layer { get; private set; }

        public void SetLayer(ImageLayer layer)
        {
            Layer = layer;
        }

        public override void ItemStarted(TrackItem sender, TimingEventArgs e)
        {
            if (Layer != null)
            {
                var rootCanvas = Layer.Parent as Canvas;

                Layer.Source = new BitmapImage(new Uri(sender.OriginalPath));

                Layer.Width = rootCanvas.ActualWidth * sender.ItemProperty.Size;
                Layer.Height = rootCanvas.ActualHeight * sender.ItemProperty.Size;

                Canvas.SetLeft(Layer, (rootCanvas.ActualWidth - Layer.Width + (rootCanvas.ActualWidth * 2 * sender.ItemProperty.PositionX)) / 2);
                Canvas.SetTop(Layer, (rootCanvas.ActualHeight - Layer.Height + (rootCanvas.ActualHeight * 2 * sender.ItemProperty.PositionY)) / 2);

                sender.ItemProperty.PropertyChanged += (sen, ev) =>
                {
                    switch (ev.ChangedProperty.ToLower())
                    {
                        case "opacity":
                            Layer.Opacity = sender.ItemProperty.Opacity;
                            break;

                        case "positionx":
                        case "positiony":
                        case "size":
                            Layer.Width = rootCanvas.ActualWidth * sender.ItemProperty.Size;
                            Layer.Height = rootCanvas.ActualHeight * sender.ItemProperty.Size;

                            Canvas.SetLeft(Layer, (rootCanvas.ActualWidth - Layer.Width + (rootCanvas.ActualWidth * 2 * sender.ItemProperty.PositionX)) / 2);
                            Canvas.SetTop(Layer, (rootCanvas.ActualHeight - Layer.Height + (rootCanvas.ActualHeight * 2 * sender.ItemProperty.PositionY)) / 2);
                            break;
                        case "chromakeyusage":
                        case "chromakeycolor":
                        case "chromakeyenabled":
                            if (sender.ItemProperty.ChromaKeyEnabled)
                            {
                                sender.ItemProperty.ChromaKeyColor.ToHSL(out double _h, out double _s, out double _l);

                                Layer.Effect = new ChromaKeyEffect()
                                {
                                    HueMin = (float)_h,
                                    HueMax = (float)_h,
                                    SaturationMax = (float)_s,
                                    SaturationMin = (float)_s,
                                    LuminanceMax = (float)_l,
                                    LuminanceMin = (float)_l,
                                    Smooth = (float)sender.ItemProperty.ChromaKeyUsage,
                                };
                            }
                            else
                            {
                                Layer.Effect = null;
                            }
                            break;
                    }
                };
            }
                
        }

        public override void ItemEnded(TrackItem sender, TimingEventArgs e)
        {
            if (Layer != null)
                Layer.Source = null;
        }

        public override void ItemPlaying(TrackItem sender, TimingEventArgs e)
        {
        }

        public override void ItemReady(TrackItem sender, TimingReadyEventArgs e)
        {
        }

        public override void TimeLineStopped()
        {
        }
    }
}
