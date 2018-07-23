using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Unosquare.FFME;
using Unosquare.FFME.Platform;

namespace Delight
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");

            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture;

            MediaElement.FFmpegDirectory = @"c:\ffmpeg";
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ThreadPool.QueueUserWorkItem((s) =>
            {
                try
                {
                    // Force loading
                    MediaElement.LoadFFmpeg();
                }
                catch (Exception ex)
                {
                    GuiContext.Current?.EnqueueInvoke(() =>
                    {

                        MessageBox.Show(MainWindow,
                            $"FFmpeg 라이브러리들을 다음의 경로에서 불러오는 데에 실패했습니다.:\r\n    {MediaElement.FFmpegDirectory}" +
                            $"\r\n{ex.GetType().Name}: {ex.Message}\r\n\r\n프로그램을 종료합니다..",
                            "FFmpeg 오류",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);

                        Application.Current?.Shutdown();
                    });
                }
            });
        }
    }
}
