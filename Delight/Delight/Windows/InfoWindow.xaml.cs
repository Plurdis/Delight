using System;
using System.Windows;
using System.Windows.Input;

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
            viewLicense.MouseDown += ViewLicense_MouseDown;
            imgClose.MouseDown += (s, e) => this.Close();
        }

        private void ViewLicense_MouseDown(object sender, MouseButtonEventArgs e)
        {
            LicenseWindow lw = new LicenseWindow();
            lw.ShowDialog();
        }

        private void DragMove(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch (InvalidOperationException)
            {
            }
            
        }
    }
}
