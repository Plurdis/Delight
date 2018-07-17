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

namespace Delight.Windows
{
    /// <summary>
    /// InfoWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class InfoWindow : Window
    {
        public InfoWindow()
        {
            InitializeComponent();
            btnNReco.Click += BtnCsCore_Click;
            btnFFmpeg.Click += BtnFFmpeg_Click;
            btnFFME.Click += BtnFFME_Click;
        }

        private void BtnFFME_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/unosquare/ffmediaelement");
        }

        private void BtnFFmpeg_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/FFmpeg/FFmpeg");
        }

        private void BtnCsCore_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/nreco/nreco");
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
