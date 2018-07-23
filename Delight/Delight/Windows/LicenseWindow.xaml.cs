using Delight.Resources;
using System;
using System.Collections.Generic;
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
    /// LicenseWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LicenseWindow : Window
    {
        public LicenseWindow()
        {
            InitializeComponent();
            lb.SelectionChanged += Lb_SelectionChanged;
            lb.SelectedIndex = 0;
        }

        string l_lgpl = ResourceManager.GetTextResource("Licenses/License_LGPL.txt");
        string l_mspl = ResourceManager.GetTextResource("Licenses/License_Ms-PL.txt");
        private void Lb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lb.SelectedItem is ListBoxItem lbItm)
            {
                string lic = lbItm.Tag.ToString();

                lic = lic.Replace("{LGPL}", l_lgpl);
                lic = lic.Replace("{Ms-PL}", l_mspl);

                licField.Text = lic;
                licField.ScrollToHome();
            }
        }
    }
}
