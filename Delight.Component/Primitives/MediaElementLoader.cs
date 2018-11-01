using Delight.Component.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Delight.Component.Primitives
{
    public class MediaElementLoader
    {
        public MediaElementLoader(MediaElementPro mediaElement)
        {
            MediaElement = mediaElement;

            mediaElement.MediaOpened += MediaElement_MediaOpened;
            mediaElement.CurrentStateChanged += MediaElement_CurrentStateChanged;
        }

        private void MediaElement_CurrentStateChanged(MediaElementPro sender, PlayerState state)
        {
            if (state == PlayerState.Closed)
            {
                IsReadyForPlay = false;
            }
        }

        private void MediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(MediaElement.Name + " Load Complete");
            MediaElement.Pause();
            IsLoaded = true;
            Console.WriteLine(IsLoaded);
        }

        MediaElementPro MediaElement { get; }

        bool IsLoaded = false;

        public bool IsReadyForPlay = false;

        public Task LoadVideo(TrackItem trackItem)
        {
            Task task = Task.Run(() =>
            {
                IsLoaded = false;
                MediaElement.Dispatcher.Invoke(() =>
                {
                    MediaElement.Tag = trackItem;
                    MediaElement.Source = new Uri(trackItem.OriginalPath, UriKind.Absolute);
                    MediaElement.Volume = 0;

                    // ===
                    MediaElement.Visibility = Visibility.Hidden;
                    // ===

                    MediaElement.Play();
                    Console.WriteLine($"Load For {MediaElement.Name} Item");
                });

                while (!IsLoaded) // 로딩이 되지 않을 때 까지
                {
                    Thread.Sleep(10);
                }
                MediaElement.Dispatcher.Invoke(() =>
                {
                    Console.WriteLine($"Load Complete {MediaElement.Name} Item");
                });

                IsReadyForPlay = true;
            });

            Console.WriteLine("Loader LoadVideo 종료 (inner)");

            return task;
        }

    }
}
