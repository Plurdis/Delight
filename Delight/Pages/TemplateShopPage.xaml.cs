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
            btnStartPacking.Click += BtnStartPacking_Click;
            //btnDownload.Click += BtnDownload_Click;
            //cb.SelectionChanged += Cb_SelectionChanged;
        }

        private void BtnStartPacking_Click(object sender, RoutedEventArgs e)
        {
            DelightTemplate template = new DelightTemplate();

            template.ConvertToMedia();

            template.DeployingPositions = GlobalViewModel.MainWindowViewModel.TimeLine.ExportData();
            template.Sources = GlobalViewModel.MainWindowViewModel.MediaItems.Where(i => i.Checked).ToList();

            template.Pack("");
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
