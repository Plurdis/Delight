using Delight.Common;
using Delight.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

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
            {
                switch (MediaTools.GetMediaTypeFromFile(location))
                {
                    case Media.MediaTypes.Unknown:
                        break;
                    case Media.MediaTypes.Image:
                        lbItem.Items.Add(new TemplateItem()
                        {
                            Content = new FileInfo(location).Name,
                            Description = "File",
                            Source = new BitmapImage(new Uri(location))
                        });
                        break;
                    case Media.MediaTypes.Sound:
                        break;
                    case Media.MediaTypes.Video:
                        break;
                    default:
                        break;
                }
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
