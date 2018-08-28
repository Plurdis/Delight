using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Delight.Common;
using Delight.Components;
using Delight.Components.Common;
using Delight.Components.Medias;
using Delight.Controls;
using Delight.Extensions;
using Delight.Timing;
using Delight.Windows;

using NReco.VideoInfo;

using DelightImage = Delight.Components.Medias.Image;

namespace Delight
{
    public partial class MainWindow
    {
        private void ExitCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void OpenFileExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (MediaTools.GetMediaFiles(out string[] locations))
                locations.ForEach(i => AddItem(i));
        }
        
        public void AddEffect(ImageSource img, string name)
        {
            lbItem.Items.Add(new TemplateItem()
            {
                StageComponent = new Effect()
                {
                    Identifier = name,
                    Thumbnail = img,
                    Time = TimeSpan.FromSeconds(10),
                },
                Source = img,
                ItemName = name,
            });
                
        }

        private void AddItem(string location)
        {
            if (!File.Exists(location))
                return;

            var fi = new FileInfo(location);
            ImageSource image;
            switch (MediaTools.GetMediaTypeFromFile(location))
            {
                case MediaTypes.Unknown:
                    break;
                case MediaTypes.Image:
                    image = new BitmapImage(new Uri(location));
                    lbItem.Items.Add(new TemplateItem()
                    {
                        //Content = fi.Name,
                        //ItemName = "Local Image File",
                        ItemName = fi.Name,
                        Source = image,
                        StageComponent = new DelightImage()
                        {
                            Identifier = fi.Name,
                            OriginalPath = location,
                            Time = TimeSpan.FromSeconds(20),
                            Thumbnail = image,
                        },
                    });
                    break;
                case MediaTypes.Sound:
                    break;
                case MediaTypes.Video:
                    image = MediaTools.GetMediaThumbnail(location);
                    lbItem.Items.Add(new TemplateItem()
                    {
                        //Content = fi.Name,
                        //ItemName = "Local Video File",
                        ItemName = fi.Name,
                        Source = image,
                        StageComponent = new Video()
                        {
                            Identifier = fi.Name,
                            OriginalPath = location,
                            Time = MediaTools.GetMediaDuration(location),
                            Thumbnail = image,
                        },
                        //ToolTip = new ItemToolTip()
                        //{
                        //    Image = image,
                        //    Text= "Test",
                        //},
                    });
                    break;
                default:
                    break;
            }
        }


        private void NewProjectExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            // 새로운 프로젝트
        }

        private void OpenProjectExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (MediaTools.GetProjectFile(out string location))
            {
                MessageBox.Show(location);
            }
        }

        private void SaveAsExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            // 다른 이름으로 저장
        }

        private void SaveExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            // 해당 이름으로 저장
        }

        private void ExportExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            // 내보내기
        }

        private void ViewInfoExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            InfoWindow wdw = new InfoWindow();
            wdw.ShowDialog();
        }
        
        private void PlayExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (tl.IsReady)
                return;
            if (!tl.IsRunning)
            {
                tl.Play();
                //pw?.player1.Play();
            }
            else
            {
                tl.Stop();
                
                pw?.player1.Pause();
            }
        }

        private void ManageTemplateExecuted(object sender, ExecutedRoutedEventArgs e)
        {
#if DEBUG
            TemplateWindow tw = new TemplateWindow();
            var titm = tw.ShowDialog();
            if (titm != null)
            {
                testItem.Visibility = Visibility.Visible;
                var img = new BitmapImage(new Uri("pack://application:,,,/Delight;component/Resources/Template/대나무숲_나비.png", UriKind.Absolute));
                AddEffect(img, "요소 - 나비");
                img = new BitmapImage(new Uri("pack://application:,,,/Delight;component/Resources/Template/대나무숲_카메라 이동.png", UriKind.Absolute));
                AddEffect(img, "이동 - 카메라");
                img = new BitmapImage(new Uri("pack://application:,,,/Delight;component/Resources/Template/대나무숲_하늘 씬.png", UriKind.Absolute));
                AddEffect(img, "장면 - 하늘 장면");
                img = new BitmapImage(new Uri("pack://application:,,,/Delight;component/Resources/Template/대나무숲 자료.png", UriKind.Absolute));
                AddEffect(img, "장면 - 대나무 장면");
            }
#else

#endif
        }

        //#if DEBUG

        private void PlayWindowVisibleExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (pw.Visibility == Visibility.Visible)
            {
                pw.Hide();
            }
            else
            {
                pw.Show();
            }
        }

        private void UnityPreviewVisibleCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (bg.Visibility == Visibility.Visible)
            {
                bg.Visibility = Visibility.Hidden;
            }
            else
            {
                bg.Visibility = Visibility.Visible;
            }
        }

//#endif
    }
}
