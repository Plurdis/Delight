using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using Delight.Components.Common;
using Delight.Controls;
using Delight.Extensions;
using Delight.LogManage;

namespace Delight.TimeLineComponents
{
    public class TimeLineReader
    {
        public TimeLineReader(TimeLine timeLine)
        {
            _timeLine = timeLine;

            _timeLine.FrameChanged += _timeLine_FrameChanged;
            _timeLine.FrameMouseChanged += (s, e) => StopLoad();

            _timeLine.ItemAdded += _timeLine_ItemAdded;
        }

        public void SetPlayer(MediaElementPro player1, MediaElementPro player2)
        {
            if (player1 == player2)
                throw new Exception("player1와 player2는 같은 인스턴스 일 수 없습니다.");

            if (player1 != null)
            {
                player1.MediaOpened -= Player_MediaOpened;
            }
            if (player2 != null)
            {
                player2.MediaOpened -= Player_MediaOpened;
            }

            this.player1 = player1;
            this.player2 = player2;

            player1.MediaOpened += Player_MediaOpened;
            player1.CurrentStateChanged += Player_CurrentStateChanged;
            player2.MediaOpened += Player_MediaOpened;
            player2.CurrentStateChanged += Player_CurrentStateChanged;
        }

        private void Player_CurrentStateChanged(MediaElementPro sender, Common.PlayerState state)
        {
            
        }

        public bool IsPlaying { get; private set; } = false;

        public void PrintLoadVideosInfo()
        {
            Console.WriteLine("------------------------------------");
            Console.WriteLine("            Loaded Videos           ");
            Console.WriteLine("------------------------------------");

            int i = 1;
            foreach(TrackItem item in _allVideos)
            {
                Console.WriteLine(i++ + " Item");
                Console.WriteLine("Location : " + item.OriginalPath);
                Console.WriteLine("StartPosition : " + item.Offset);
                Console.WriteLine("------------------------------------");
            }
        }

        Queue<TrackItem> _allVideos = new Queue<TrackItem>();
        Queue<TrackItem> _loadWaitVideos = new Queue<TrackItem>();
        MediaElementPro player1, player2;

        public bool PlayerAssigned => (player1 != null) && (player2 != null);

        bool loading = false;

        private void _timeLine_ItemAdded(object sender, ItemEventArgs e)
        {
            if (e.Item.TrackType == TrackType.Video)
            {
                var item = e.Item;

                _allVideos.Enqueue(item);
                PrintLoadVideosInfo();
            }
        }

        public void StartLoad()
        {
            loading = true;

            _allVideos.Clear();

            _timeLine.Dispatcher.Invoke(() =>
            {
                foreach (TrackItem item in _timeLine.GetItems(TrackType.Video, _timeLine.Position).OrderBy(i => i.Offset))
                {
                    _allVideos.Enqueue(item);
                }

                LoadCheck();
            });

            LoadWaitingVideos();
        }

        public void StopLoad()
        {
            loading = false;
            player1.Close();
            player2.Close();
        }

        public void SwitchPlayer(bool showPlayer1)
        {
            if (showPlayer1)
            {
                player1.Visibility = Visibility.Visible;
                player2.Visibility = Visibility.Hidden;
            }
            else
            {
                player2.Visibility = Visibility.Visible;
                player1.Visibility = Visibility.Hidden;
            }
        }

        public IEnumerable<TrackItem> GetTrackItem(int frame)
        {
            return _timeLine.Items.Where(i => i.Offset <= frame && i.Offset + i.FrameWidth > frame);
        }

        private void _timeLine_FrameChanged(object sender, EventArgs e)
        {
            if (!PlayerAssigned)
                throw new NullReferenceException("player1 또는 player2가 할당되지 않았습니다. SetPlayer 함수를 이용해서 할당해주세요.");

            if (loading)
            {
                LoadCheck();
                LoadWaitingVideos();
                if (_timeLine.Tag == null)
                    return;

                if (_timeLine.Position == player1.GetTag<TrackItem>().Offset)
                    player1.Play();
            }
        }

        bool player1Playing = false;
        bool loadComplete = false;
        private void LoadWaitingVideos()
        {
            if (_loadWaitVideos.Count == 0)
                return;
            if (!player1Playing)
            {
                player1.Dispatcher.Invoke(() =>
                {
                    player1.Tag = _loadWaitVideos.Dequeue();
                    player1.Source = new Uri(player1.GetTag<TrackItem>().OriginalPath, UriKind.Absolute);
                    player1.Play();
                });
            }
            else
            {
                player2.Dispatcher.Invoke(() =>
                {

                    player2.Tag = _loadWaitVideos.Dequeue();
                    player2.Source = new Uri(player2.GetTag<TrackItem>().OriginalPath, UriKind.Absolute);
                    player2.Play();

                });
            }

            while (!loadComplete)
            {
            }
            Console.WriteLine("Complete");
        }

        private void Player_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (sender == player1)
            {
                player1.Position = 
                    MediaTools.FrameToTimeSpan(player1.GetTag<TrackItem>().ForwardOffset, _timeLine.FrameRate);
                
                player1.Pause();
            }
            else if (sender == player2)
            {
                player2.Position = 
                    MediaTools.FrameToTimeSpan(player2.GetTag<TrackItem>().ForwardOffset, _timeLine.FrameRate);
                player2.Pause();
            }

            loadComplete = true;
        }

        private void LoadCheck()
        {
            if (_allVideos.Count == 0)
                return;

            TrackItem item = _allVideos.Peek();
            if ((item.Offset - _timeLine.Position) < MediaTools.TimeSpanToFrame(TimeSpan.FromSeconds(10), _timeLine.FrameRate))
            {
                Console.WriteLine("Should be Load!" + item.OriginalPath);

                _loadWaitVideos.Enqueue(item);

                _allVideos.Dequeue();
            }
        }

        TimeLine _timeLine;
    }
}
