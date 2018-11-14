using Delight.Component.Common;
using Delight.Component.Converters;
using Delight.Component.Extensions;
using Delight.Core.Sources;
using Delight.Core.Stage.Components.Media;
using Delight.Core.Template;
using Delight.ViewModel;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
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

            tcSelectedIndex.SelectionChanged += TcSelectedIndex_SelectionChanged;
            //btnDownload.Click += BtnDownload_Click;
            //cb.SelectionChanged += Cb_SelectionChanged;
        }

        private void TcSelectedIndex_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tcMenu.SelectedIndex = (new StringToIntConverter()).Convert(((ListBoxItem)tcSelectedIndex.SelectedItem).GetTag<string>(), typeof(string), null, null);
            tcContent.SelectedIndex = (new StringToIntConverter()).Convert(((ListBoxItem)tcSelectedIndex.SelectedItem).GetTag<string>(), typeof(string), null, null);
        }

        private void BtnStartPacking_Click(object sender, RoutedEventArgs e)
        {
            var checkedItem = GlobalViewModel.MainWindowViewModel.MediaItems.Where(i => i.Checked);

            if (checkedItem.Count() == 0)
            {
                MessageBox.Show("패킹할 아이템이 없습니다");
                return;
            }

            DelightTemplate template = new DelightTemplate();

            if (cbPacking.IsChecked.Value)
            {
                template.DeployingPositions = GlobalViewModel.MainWindowViewModel.TimeLine.ExportData();
            }
            template.Sources = checkedItem.Select(i =>
            {
                return DelightTemplate.ConvertToSource(i);
            }).ToList();

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            sfd.Filter = "Delight 템플릿 패키지 파일 (*.dlpack)|*.dlpack";
            if (sfd.ShowDialog().Value)
            {
                template.Pack(sfd.FileName);
            }
        }

        ObservableCollection<BaseSource> list;

        public void InitializeViewModel()
        {

            list = new ObservableCollection<BaseSource>();
            this.DataContext = GlobalViewModel.TemplateShopViewModel;

            foreach (FileInfo fi in new DirectoryInfo(@"C:\Users\uutak\바탕 화면\테스트 템플릿 모음").GetFiles())
            {
                list.Add(new YoutubeSource(Path.GetFileNameWithoutExtension(fi.Name), fi.FullName, "a"));
            }
            
            templates.ItemsSource = list;
            projectItems.ItemsSource = GlobalViewModel.MainWindowViewModel.MediaItems;
        }

        private void CheckBox_SourceUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
        {
            tbTemplate.Text = new ComponentsToTextConverter().Convert(GlobalViewModel.MainWindowViewModel.MediaItems, null, null, null);
        }
    }
}
