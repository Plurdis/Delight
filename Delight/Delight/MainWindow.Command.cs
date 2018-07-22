using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Delight.Components.Common;
using Delight.Components.Medias;
using Delight.Controls;

using NReco.VideoConverter;
using NReco.VideoInfo;
using DelightImage = Delight.Components.Medias.Image;

namespace Delight
{
    public partial class MainWindow
    {
        public void ExitCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        public void OpenFileExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (MediaTools.GetMediaFile(out string location))
                AddItem(location);
        }

        public void AddItem(string location)
        {
            var fi = new FileInfo(location);

            switch (MediaTools.GetMediaTypeFromFile(location))
            {
                case MediaTypes.Unknown:
                    break;
                case MediaTypes.Image:
                    
                    lbItem.Items.Add(new TemplateItem()
                    {
                        Content = fi.Name,
                        Description = "Local Image File",
                        Source = new BitmapImage(new Uri(location)),
                        StageComponent = new DelightImage()
                        {
                            Identifier = fi.Name,
                            OriginalPath = location,
                            Time = TimeSpan.FromSeconds(20),
                        },
                    });
                    break;
                case MediaTypes.Sound:
                    break;
                case MediaTypes.Video:
                    Stream stream = new MemoryStream();
                    MediaInfo info = MediaTools.GetMediaInfo(location);

                    (new FFMpegConverter()).GetVideoThumbnail(location, stream);
                    lbItem.Items.Add(new TemplateItem()
                    {
                        Content = fi.Name,
                        Source = MediaTools.GetImageFromStream(stream),
                        Description = "Local Video File",
                        StageComponent = new Video()
                        {
                            Identifier = fi.Name,
                            OriginalPath = location,
                            Time = MediaTools.GetMediaDuration(location),
                        }
                    });

                    stream.Dispose();
                    break;
                default:
                    break;
            }
        }


        public void NewProjectExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            // 새로운 프로젝트
        }

        public void OpenProjectExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (MediaTools.GetProjectFile(out string location))
            {
                MessageBox.Show(location);
            }
        }

        public void SaveAsExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            // 다른 이름으로 저장
        }

        public void SaveExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            // 해당 이름으로 저장
        }

        public void ExportExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            // 내보내기
        }
    }
}
