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
using Delight.Extensions;

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
                    break;
                }
            }

            this.Topmost = true;
            this.Activate();

            this.Loaded += PlayWindow_Loaded;
        }

        private void PlayWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow mw = (MainWindow)System.Windows.Application.Current.MainWindow;

            mw.bg.Background = new VisualBrush(rootElement);
            
            UnityContainerLoader loader = new UnityContainerLoader(@"C:\Users\uutak\바탕 화면\UnityTest\UnityTest.exe", this, control);

            //Thread thr = new Thread(() =>
            //{
            //    while (true)
            //    {
            //        Dispatcher.Invoke(() =>
            //        {
                        
            //            testImage.Source = WinFormControlToImage(control);
                        
            //        });
                    
            //        Thread.Sleep(1000 / 24);
            //    }
            //});

            //thr.Start();
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
