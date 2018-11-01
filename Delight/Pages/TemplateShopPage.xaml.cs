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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Delight.Pages
{
    /// <summary>
    /// TemplateShopPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TemplateShopPage : UserControl
    {
        public TemplateShopPage()
        {
            InitializeComponent();
            btnDownload.Click += BtnDownload_Click;
        }

        private void BtnDownload_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
