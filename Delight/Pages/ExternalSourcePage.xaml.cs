using Delight.Component.Common;
using Delight.Component.Converters;
using Delight.Component.Extensions;
using Delight.Core.Sources;
using Delight.Core.Stage.Components.Media;
using Delight.Core.Template;
using Delight.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Delight.Pages
{
    /// <summary>
    /// ExternalSourcePage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ExternalSourcePage : UserControl
    {
        public ExternalSourcePage()
        {
            InitializeComponent();
            InitializeViewModel();
            btnDownload.Click += BtnDownload_Click;
            cb.SelectionChanged += Cb_SelectionChanged;

            btnCheck.Click += BtnCheck_Click;
            btnBack.Click += BtnBack_Click;

            tcSelectedIndex.SelectionChanged += TcSelectedIndex_SelectionChanged;
        }

        private void TcSelectedIndex_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //tcMenu.SelectedIndex = (new StringToIntConverter()).Convert(((ListBoxItem)tcSelectedIndex.SelectedItem).GetTag<string>(), typeof(string), null, null);
            tcContent.SelectedIndex = (new StringToIntConverter()).Convert(((ListBoxItem)tcSelectedIndex.SelectedItem).GetTag<string>(), typeof(string), null, null);
        }

        private void BtnCheck_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                YoutubeSource ys = YoutubeDownloader.GetYoutubeSource_Offical(tbSearchLink.Text);

                GlobalViewModel.ExternalSourceViewModel.Sources.Insert(0, ys);
                tcSelectedIndex.SelectedIndex = 0;
                templates.SelectedIndex = 0;
            }
            catch (Exception)
            {
                MessageBox.Show("링크가 잘못되었습니다. 확인 후 다시 시도해주세요.");
            }

            tbSearchLink.Clear();

        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            GlobalViewModel.MainWindowViewModel.ViewingIndex = 0;
        }

        private void Cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cb.Items.Count == 0)
                return;

            if (cb.SelectedIndex != 0)
                cb.SelectedIndex = 0;
        }

        public void InitializeViewModel()
        {
            this.DataContext = GlobalViewModel.ExternalSourceViewModel;
            templates.ItemsSource = GlobalViewModel.ExternalSourceViewModel.Sources;
        }

        public BaseSource DownloadingSource;

        private void BtnDownload_Click(object sender, RoutedEventArgs e)
        {
            if (templates.SelectedItem is YoutubeSource source)
            {
                DownloadingSource = source;
                if (source.Download(cb.SelectedIndex))
                {
                    source.DownloadProgressChanged += Source_DownloadProgressChanged;
                    source.DownloadFileCompleted += Source_DownloadFileCompleted;
                }
                else
                {
                    Source_DownloadFileCompleted(null, null);
                }
            }
        }

        private void Source_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            var item = FileCacheDictionary.GetPathFromId(DownloadingSource.Id);

            string path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Delight", item.Value.Item1);

            GlobalViewModel.MainWindowViewModel.MediaItems.Add(new VideoMedia()
            {
                Identifier = item.Key,
                Time = MediaTools.GetMediaDuration(path),
                Path = path,
                Thumbnail = new Uri(DownloadingSource.ThumbnailUri),
                FromYoutube = true,
                DownloadID = DownloadingSource.Id,
                Id = DownloadingSource.Id,
            });
        }

        private void Source_DownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            GlobalViewModel.MainWindowViewModel.BottomText = $"유튜브 영상을 다운로드 중입니다. ({e.ProgressPercentage}%)";
        }
    }
}
