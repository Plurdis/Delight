using Delight.Controls;
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
    /// TemplateWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TemplateWindow : Window
    {
        public TemplateWindow()
        {
            InitializeComponent();
        }

        public new TemplateItem ShowDialog()
        {
            base.ShowDialog();

            if (_completeClosed)
                return _selectedItem;

            return null;
        }

        private bool _completeClosed;

        private TemplateItem _selectedItem;

        private void TemplateItem_Selected(object sender, RoutedEventArgs e)
        {
            if (sender is TemplateItem itm)
            {
                _selectedItem = itm;

                img.Source = itm.Source;
                tbName.Text = itm.ItemName;
                tbDescription.Text = itm.Tag.ToString();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _completeClosed = true;
            this.Close();
        }
    }
}
