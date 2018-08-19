﻿using System;
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
using Delight.Components.Common;
using Delight.Components.Medias;
using Delight.Controls;
using Delight.TimeLineComponents;
using Delight.Windows;

using NReco.VideoConverter;
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
            if (MediaTools.GetMediaFile(out string location))
                AddItem(location);
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
                        ToolTip = new ItemToolTip()
                        {
                            Image = image,
                            Text= "Test",
                        },
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
