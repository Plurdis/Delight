using Delight.Component;
using Delight.Component.Controls;
using Delight.Component.Layers;
using Delight.Component.Primitives.Controllers;
using Delight.Component.Primitives.TimingReaders;
using Delight.Core.Stage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Delight.Windows
{
    /// <summary>
    /// PlayWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PlayWindow : Window
    {
        public PlayWindow()
        {
            InitializeComponent();
            this.MouseLeftButtonDown += (s, e) => this.DragMove();
            foreach (Screen scr in Screen.AllScreens)
            {
                if (!scr.Primary)
                {
                    this.Left = scr.Bounds.Left;
                    this.Top = scr.Bounds.Top;
                    this.Width = scr.Bounds.Width;
                    this.Height = scr.Bounds.Height;

                    rootElement.Width = scr.Bounds.Width;
                    rootElement.Height = scr.Bounds.Height;
                    //testElement.Width = scr.Bounds.Width;
                    //testElement.Height = scr.Bounds.Height;
                    break;
                }
            }
            this.Topmost = true;
            this.Activate();

            this.Loaded += PlayWindow_Loaded;

            Layers = new List<ILayer>();

            List<UIElement> itm = rootElement.Children.Cast<UIElement>().ToList();
        }

        TimeLine _timeLine;
        VisualBrush _brush;

        public void ConnectTimeLine(TimeLine timeLine)
        {
            if (timeLine != null)
            {
                if (_timeLine != null)
                {
                    _timeLine.TrackAdded -= TimeLine_TrackAdded;
                    _timeLine.TrackRemoved -= TimeLine_TrackRemoved;
                }
                timeLine.TrackAdded += TimeLine_TrackAdded;
                timeLine.TrackRemoved += TimeLine_TrackRemoved;

                _timeLine = timeLine;

                _timeLine.FrameChanged += (s, e) =>
                {

                };
            }
        }

        private bool IsVisualSourceType(SourceType sourceType)
        {
            switch (sourceType)
            {
                case SourceType.Image:
                case SourceType.Video:
                    return true;
                default:
                    return false;
            }
        }

        private void TimeLine_TrackAdded(object sender, TrackEventArgs e)
        {
            if (sender is TimeLine tl)
            {
                if (IsVisualSourceType(e.SourceType))
                {
                    if (e.SourceType == SourceType.Video)
                    {
                        var vLayer = new VideoLayer(e.Track);
                        vLayer.Loaded += (s, ev) =>
                        {
                            if (tl.TimingReaders[e.Track] is DelayTimingReader dReader)
                            {
                                dReader.SetPlayer(vLayer.Player1, vLayer.Player2);
                            }
                            ((VideoController)tl.Controllers[e.Track]).SetPlayer(vLayer.Player1, vLayer.Player2);
                        };

                        rootElement.Children.Add(vLayer);
                    }
                    else if (e.SourceType == SourceType.Image)
                    {
                        var iLayer = new ImageLayer(e.Track);
                        iLayer.Loaded += (s, ev) =>
                        {
                            ((ImageController)tl.Controllers[e.Track]).SetLayer(iLayer);
                        };

                        rootElement.Children.Add(iLayer);
                    }
                    UpdateZIndex();
                }
            }
        }

        public void UpdateZIndex()
        {
            var items = rootElement.Children.Cast<BaseLayer>();
            int i = items.Count();
            foreach (BaseLayer layer in items)
            {
                int zindex = i - _timeLine.GetVisualTrackIndex(layer.Track);
                Canvas.SetZIndex(layer, zindex);
            }
        }

        private void TimeLine_TrackRemoved(object sender, TrackEventArgs e)
        {
        }

        private List<ILayer> Layers { get; }

        private void AddLayer(ILayer layer)
        {
            if (!Layers.Contains(layer))
            {
                Layers.Add(layer);
            }
            if (!rootElement.Children.Contains((UIElement)layer))
            {
                rootElement.Children.Add((UIElement)layer);
            }
        }

        private void RemoveScene(ILayer scene)
        {
            if (Layers.Contains(scene))
            {
                Layers.Remove(scene);
            }
            if (rootElement.Children.Contains((UIElement)scene))
            {
                rootElement.Children.Remove((UIElement)scene);
            }
        }

        private void PlayWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //MainWindow mw = (MainWindow)System.Windows.Application.Current.MainWindow;

            _brush = new VisualBrush(rootElement)
            {
                AlignmentX = AlignmentX.Center,
                AlignmentY = AlignmentY.Center,
                ViewportUnits = BrushMappingMode.RelativeToBoundingBox,
                Stretch = Stretch.Uniform,

            };

            //mw.bg.Background = _brush;

            //_timeLine.FrameChanged += 
        }

        internal void ConnectPreview(Rectangle preview)
        {
            preview.Height = this.Height / (this.Width / preview.ActualWidth);
            preview.SizeChanged += (s, ev) =>
            {
                preview.Height = this.Height / (this.Width / preview.ActualWidth);
            };
            preview.Fill = new VisualBrush(rootElement);
        }
    }
}
