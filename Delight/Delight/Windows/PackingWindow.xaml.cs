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
    /// PackingWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PackingWindow : Window
    {
        public PackingWindow()
        {
            InitializeComponent();
            SyncWithFiles(Application.Current.MainWindow as MainWindow);
            btnPacking.Click += BtnPacking_Click;
            btnCancel.Click += BtnCancel_Click;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnPacking_Click(object sender, RoutedEventArgs e)
        {
            (new LoadingWindow()).ShowDialog();
            this.Close();
        }

        public void SyncWithFiles(MainWindow mw)
        {
            TimeLine tl = mw.tl;

            IEnumerable<TemplateItem> templateItems = mw.lbItem.Items.Cast<TemplateItem>();

            foreach(TemplateItem itm in templateItems)
            {
                switch (itm.StageComponent.TrackType)
                {
                    case Timing.TrackType.Video:
                        tvVideos.AddItem(itm.ItemName);
                        break;
                    case Timing.TrackType.Sound:
                        tvSounds.AddItem(itm.ItemName);
                        break;
                    case Timing.TrackType.Image:
                        tvImages.AddItem(itm.ItemName);
                        break;
                }
            }

            int i = 1;
            foreach (Track t in tl.VisualTracks)
            {
                tvVisibleTracks.AddItem($"{i++}번 트랙 ({t.TrackTypeText})");
            }
            i = 1;
            foreach (Track t in tl.NotVisualTracks)
            {
                tvInVisibleTracks.AddItem($"{i++}번 트랙 ({t.TrackTypeText})");
            }
        }
    }

    public static class TreeViewItemEx
    {
        public static void AddItem(this TreeViewItem itm, string text)
        {
            itm.Items.Add(new CheckBox() { Content = text });
        }
    }
}
