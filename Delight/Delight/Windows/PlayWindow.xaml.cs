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

            this.MouseDown += (s, e) => this.DragMove();

            player.Open(new Uri(@"D:\도깨비\[tvN] 도깨비.E03.161209.720p-NEXT.mp4", UriKind.Absolute));
            //player.MediaEnded += (s, e) => { player.Close(); };

            foreach(Screen scr in Screen.AllScreens)
            {
                if (!scr.Primary)
                {
                    this.Left = scr.Bounds.Left;
                    this.Top = scr.Bounds.Top;
                    break;
                }
            }

            MainWindow mw = (MainWindow)System.Windows.Application.Current.MainWindow;

            this.Topmost = true;
            this.Activate();

            mw.preview.Fill = new VisualBrush(rootElement);

            //this.Loaded += (s, e) => this.Hide();
        }
    }
}
