using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StagePainter.Windows
{
    /// <summary>
    /// InfoWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class InfoWindow : Window
    {
        public InfoWindow()
        {
            InitializeComponent();
            btnCsCore.Click += BtnCsCore_Click;
            btnFFmpeg.Click += BtnFFmpeg_Click;
        }

        private void BtnFFmpeg_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://www.ffmpeg.org/");
        }

        private void BtnCsCore_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/filoe/cscore");
        }
    }
}
