using Delight.Component.Common;
using Delight.Component.Converters;
using Delight.Core.Sources;
using Delight.Core.Stage.Components.Media;
using Delight.Core.Template;
using Delight.ViewModel;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
            InitializeViewModel();
            //btnDownload.Click += BtnDownload_Click;
            //cb.SelectionChanged += Cb_SelectionChanged;
        }

        public void InitializeViewModel()
        {
            this.DataContext = GlobalViewModel.ExternalSourceViewModel;
            templates.ItemsSource = GlobalViewModel.ExternalSourceViewModel.Sources;
            projectItems.ItemsSource = GlobalViewModel.MainWindowViewModel.MediaItems;
        }

        private void CheckBox_SourceUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
        {
            tbTemplate.Text = new ComponentsToTextConverter().Convert(GlobalViewModel.MainWindowViewModel.MediaItems, null, null, null);
        }
    }
}
