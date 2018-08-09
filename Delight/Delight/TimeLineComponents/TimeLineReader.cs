using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Delight.Common;
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
            TimeLine = timeLine;

            TimeLine.FrameChanged += TimeLine_FrameChanged;
            TimeLine.FrameMouseChanged += (s, e) => StopLoad();

            TimeLine.ItemAdded += TimeLine_ItemAdded;
        }

        public void SetPlayer(MediaElementPro player1, MediaElementPro player2)
        {
            if (player1 == player2)
                throw new Exception("player1와 player2는 같은 인스턴스 일 수 없습니다.");

            this.player1 = player1;
            this.player2 = player2;

            loader1 = new MediaElementLoader(player1);
            loader2 = new MediaElementLoader(player2);
            
            player1.CurrentStateChanged += Player_CurrentStateChanged;
            player2.CurrentStateChanged += Player_CurrentStateChanged;

            StopLoad();
        }

        Queue<TrackItem> _allVideos = new Queue<TrackItem>();
        Queue<TrackItem> _loadWaitVideos = new Queue<TrackItem>();
        MediaElementPro player1, player2;
        MediaElementLoader loader1, loader2;

        public bool IsPlaying => 
            player1?.CurrentState == PlayerState.Playing || player2?.CurrentState == PlayerState.Playing;

        public bool PlayerAssigned => (player1 != null) && (player2 != null);

        bool loading = false;

        #region [  TimeLine Event  ]

        private void TimeLine_ItemAdded(object sender, ItemEventArgs e)
        {
            if (e.Item.TrackType == TrackType.Video)
            {
                _allVideos.Enqueue(e.Item);
            }
        }

        private void TimeLine_FrameChanged(object sender, EventArgs e)
        {
            if (!PlayerAssigned)
                throw new NullReferenceException("player1 또는 player2가 할당되지 않았습니다. SetPlayer 함수를 이용해서 할당해주세요.");

            if (!TimeLine.IsRunning)
                return;

            //Console.WriteLine(MediaTools.FrameToTimeSpan(TimeLine.Position, TimeLine.FrameRate));

            if (loading)
            {
                LoadCheck();
                Task.Run(() =>
                {
                    LoadWaitingVideos();
                    
                    player1.Dispatcher.Invoke(() =>
                    {
                        MainWindow mw = Application.Current.MainWindow as MainWindow;

                        mw.Title = MediaTools.FrameToTimeSpan(TimeLine.Position, TimeLine.FrameRate).ToString();
                        if (player1.Tag == null && player2.Tag == null)
                        {
                            return;
                        }
                        if (player1.CurrentState != PlayerState.Playing &&
                            TimeLine.Position - 1 > player1.GetTag<TrackItem>().Offset)
                        {
                            player1.Position = MediaTools.FrameToTimeSpan(player1.GetTag<TrackItem>().ForwardOffset, TimeLine.FrameRate);
                            player1.Play();
                            player1.Volume = 1;
                            player1.Visibility = Visibility.Visible;
                        }
                    });
                });
            }
        }

        #endregion

        bool p1Playing = false;

        TimeLine TimeLine { get; }

        private void LoadWaitingVideos()
        {
            if (!TimeLine.IsRunning && !TimeLine.IsReady)
                return;

            if (_loadWaitVideos.Count == 0)
                return;
            if (!p1Playing)
            {
                loader1.LoadVideo(_loadWaitVideos.Dequeue()).Wait();
            }
            else
            {
                loader2.LoadVideo(_loadWaitVideos.Dequeue()).Wait();
            }
            Console.WriteLine("Loader LoadVideo 종료 (outer)");

            p1Playing = !p1Playing;
        }

        public void StartLoad()
        {
            loading = true;

            _allVideos.Clear();

            TimeLine.Dispatcher.Invoke(() =>
            {
                foreach (TrackItem item in TimeLine.GetItems(TrackType.Video, TimeLine.Position).OrderBy(i => i.Offset))
                {
                    _allVideos.Enqueue(item);
                }

                LoadCheck();
            });

            LoadWaitingVideos();

            Console.WriteLine("StartLoad Method End");
        }

        public void StopLoad()
        {
            loading = false;
            player1.Source = null;
            player2.Source = null;
            player1.Close();
            player2.Close();
            player1.Visibility = Visibility.Hidden;
            player2.Visibility = Visibility.Hidden;
            player1.Volume = 0;
            player2.Volume = 0;
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
            return TimeLine.Items.Where(i => i.Offset <= frame && i.Offset + i.FrameWidth > frame);
        }

        private void Player_CurrentStateChanged(MediaElementPro sender, PlayerState state)
        {
            if (state == PlayerState.Playing)
            {
                //sender.Position =
                //    MediaTools.FrameToTimeSpan(sender.GetTag<TrackItem>().ForwardOffset, TimeLine.FrameRate);

                //sender.Pause();

                //loadComplete = true;
            }
            else if (state == PlayerState.Ended)
            {
                "현재 재생중이던 플레이어가 종료되었습니다.".Log();
            }
            Console.WriteLine(sender.Name + " :; " + state);
        }

        private void LoadCheck()
        {
            if (_allVideos.Count == 0)
                return;

            TrackItem item = _allVideos.Peek();
            if ((item.Offset - TimeLine.Position) < MediaTools.TimeSpanToFrame(TimeSpan.FromSeconds(10), TimeLine.FrameRate))
            {
                Console.WriteLine("Should be Load!" + item.OriginalPath);

                _loadWaitVideos.Enqueue(item);

                _allVideos.Dequeue();
            }
        }
    }
}
