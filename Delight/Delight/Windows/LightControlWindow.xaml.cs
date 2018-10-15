using Delight.Common;
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
    /// LightControlWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LightControlWindow : Window
    {
        public const string URI = "pack://application:,,,/Delight;component/Resources/MovingLight";

        public LightControlWindow()
        {
            InitializeComponent();

            //dmx.Send(1, 0);
            //dmx.Send(2, 1);
            //dmx.Send(3, 1);
            //dmx.Send(4, 1);
            //dmx.Send(5, 1);
            //dmx.Send(6, 1);
            //dmx.Send(7, 1);
            //dmx.Send(8, 1);

            this.Closing += LightControlWindow_Closing;

            slX.PropertyChanged += SlX_PropertyChanged;
            slY.PropertyChanged += SlY_PropertyChanged;
            slLight.PropertyChanged += SlLight_PropertyChanged;
            slTicking.PropertyChanged += SlTicking_PropertyChanged;
        }

        private void LightControlWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }

        private void SlTicking_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            imgSelectedProperty.Source = new BitmapImage(new Uri(URI + "Ticking.png", UriKind.Absolute));
            int value = (int)slTicking.Value;

            LightControl();
        }

        private void SlLight_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            imgSelectedProperty.Source = new BitmapImage(new Uri(URI + "Color.png", UriKind.Absolute));
            int value = (int)slLight.Value;

            LightControl();
        }

        private void SlY_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            imgSelectedProperty.Source = new BitmapImage(new Uri(URI + "Y.png", UriKind.Absolute));
            int value = (int)slY.Value;

            LightControl();
        }

        private void SlX_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            imgSelectedProperty.Source = new BitmapImage(new Uri(URI + "X.png", UriKind.Absolute));
            int value = (int)slX.Value;

            LightControl();
        }

        public void LightControl()
        {
            if (!IsLoaded)
                return;
            int[] values = new int[] {
                (int)slX.Value,
                (int)slY.Value,
                (int)slLight.Value,
                (int)slTicking.Value };

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "LightController.exe";
            startInfo.Arguments = $"{1}:{values[0]} {2}:{values[1]} {3}:{values[2]} {4}:{values[3]}";
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;

            Process processTemp = new Process();
            processTemp.StartInfo = startInfo;
            processTemp.EnableRaisingEvents = true;
            try
            {
                processTemp.Start();
            }
            catch (Exception e)
            {
            }
        }
    }
}
