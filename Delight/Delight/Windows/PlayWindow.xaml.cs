using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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

using Delight.Common;

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

            //WindowsPlatform

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
                    break;
                }
            }

            this.Topmost = true;
            this.Activate();


            this.Loaded += PlayWindow_Loaded;



            //this.Loaded += (s, e) => this.Hide();
        }

        private void PlayWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow mw = (MainWindow)System.Windows.Application.Current.MainWindow;

            mw.bg.Background = new VisualBrush(rootElement);

            //RenderTargetBitmap bitmap = new RenderTargetBitmap((int)rootElement.ActualWidth, (int)rootElement.ActualHeight, 96, 96, PixelFormats.Pbgra32);

            //mw.preview.Fill = new ImageBrush(bitmap);
            //Thread thr = new Thread(() =>
            //{
            //    int i = 0;
            //    while (true)
            //    {
            //        Dispatcher.Invoke(() =>
            //        {
            //            if (mw.tl.TimeLineReader.IsPlaying)
            //            {
            //                bitmap.Render(rootElement);

            //                GC.Collect();
            //                GC.WaitForPendingFinalizers();
            //                GC.Collect();
            //            }

            //        });
            //        i++;
            //        Thread.Sleep(1000 / 24);
            //    }
            //});

            //thr.Start();
        }

    }
}
