using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Delight.Common;
using Delight.Controls;
using Delight.Extensions;
using Delight.Layer;
using Delight.Timing;

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
            
            //Console.WriteLine(sw.ElapsedMilliseconds + "ms");
            //player1.Open(stream);

            //player1.MediaOpened += (s, e) =>
            //{
            //    Console.WriteLine(sw.ElapsedMilliseconds + "ms");
            //};

            //player2.Open(new Uri(@"D:\영화\소아온\[바카-Raws] Sword Art Online #01 VFR (MX 1280x720 x264 AAC).mp4", UriKind.Absolute));
            //player2.MediaOpened += (s, e) => player2.Play();
            //player.MediaEnded += (s, e) => { player.Close(); };

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
                    testElement.Width = scr.Bounds.Width;
                    testElement.Height = scr.Bounds.Height;
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

        private bool IsVisualTrackType(TrackType trackType)
        {
            switch (trackType)
            {
                case TrackType.Image:
                case TrackType.Video:
                    return true;
                default:
                    return false;
            }
        }

        private void TimeLine_TrackAdded(object sender, TrackEventArgs e)
        {
            if (sender is TimeLine tl)
            {
                if (IsVisualTrackType(e.TrackType))
                {
                    if (e.TrackType == TrackType.Video)
                    {
                        var vLayer = new VideoLayer(e.Track);
                        vLayer.Loaded += (s, ev) =>
                        {
                            if (tl.TimingReaders[e.Track] is DelayTimingReader dReader)
                            {
                                dReader.SetPlayer(vLayer.Player1, vLayer.Player2);
                            }
                            tl.VideoControllers[e.Track].SetPlayer(vLayer.Player1, vLayer.Player2);
                        };

                        rootElement.Children.Add(vLayer);
                    }
                    else if (e.TrackType == TrackType.Image)
                    {
                        var iLayer = new ImageLayer(e.Track);
                        iLayer.Loaded += (s, ev) =>
                        {
                            tl.ImageControllers[e.Track].SetLayer(iLayer);
                        };

                        rootElement.Children.Add(iLayer);
                    }
                    else if (e.TrackType == TrackType.Sound)
                    {
                        
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
            MainWindow mw = (MainWindow)System.Windows.Application.Current.MainWindow;
            
            _brush = new VisualBrush(rootElement)
            {
                AlignmentX = AlignmentX.Center,
                AlignmentY = AlignmentY.Center,
                ViewportUnits = BrushMappingMode.RelativeToBoundingBox,
                Stretch = Stretch.Uniform,
                
            };

            mw.bg.Background = _brush;

            //_timeLine.FrameChanged += 
        }

        public void SetViewportRange()
        {
            //double translateX = -50;
            //Viewport = new Rect(-0.2, 0, 1, 1)
            _brush.Viewport = new Rect(0.2, 0, 1 - 0.2, 1);
        }

        public ImageSource WinFormControlToImage(System.Windows.Forms.Control ctrl)
        {
            IntPtr controlDC = new IntPtr(GetDC(ctrl.Handle.ToInt32()));
            Bitmap wfImage = new Bitmap(ctrl.Width, ctrl.Height);

            Graphics g = Graphics.FromImage(wfImage);
            IntPtr wfImgDC = g.GetHdc();
            
            BitBlt(controlDC, 0, 0, ctrl.Width, ctrl.Height, wfImgDC, 0, 0, SRC_COPY);
            g.ReleaseHdc(wfImgDC);
            Bitmap wfImage2 = new Bitmap(ctrl.Width, ctrl.Height, g);

            return ImageSourceForBitmap(wfImage2);
        }

        //If you get 'dllimport unknown'-, then add 'using System.Runtime.InteropServices;'
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        public ImageSource ImageSourceForBitmap(Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally { DeleteObject(handle); }
        }

        [DllImport("user32")]
        public static extern int GetDC(int hwnd);

        [DllImport("gdi32.dll")]
        private static extern int BitBlt(IntPtr targetHandle, int targetX, int targetY, int targetWidth, int targetHeight, IntPtr sourceHandle, int sourceX, int sourceY, int rasterOperation);

        [DllImport("gdi32.dll", EntryPoint = "SelectObject")]
        private static extern IntPtr SelectObject(IntPtr targetHandle, IntPtr sourceObjectHandle);

        /// <summary>
        /// SRC_COPY
        /// </summary>
        private const int SRC_COPY = 0xcc0020;

    }
}
