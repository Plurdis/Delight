using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using Delight.Common;
using Delight.Components.Common;
using Delight.Controls;
using Delight.Core.Common;
using Delight.Core.Extension;
using Delight.Core.Extensions;
using Delight.Extensions;
using Delight.Timing.Common;
using Delight.Timing.Controller;

#pragma warning disable CS0067

namespace Delight.Timing
{
    // TODO: 코드 정리
    /// <summary>
    /// 미리 타이밍에 대한 정보를 받는 <see cref="TimingReader"/> 입니다.
    /// </summary>
    public class DelayTimingReader : TimingReader
    {
        public event TimingReadyDelegate ItemReady;

        public DelayTimingReader(TimeLine timeLine, Track track) : base(timeLine, track)
        {
            TimeLine.FrameChanged += TimeLine_FrameChanged;

            // DelayTimingReader에서만 사용 -> 미리 로드된 아이템을 모두 없앰
            TimeLine.FrameMouseChanged += (s, e) => StopLoad();

            // DelayTimingReader에서만 사용 -> 아이템이 추가될때는 전체 아이템에 추가
            TimeLine.ItemAdded += TimeLine_ItemAdded;
        }

        public void SetPlayer(MediaElementPro player1, MediaElementPro player2)
        {
            if (player1 == player2)
                throw new Exception("player1와 player2는 같은 인스턴스 일 수 없습니다.");

            this.player1 = player1;
            this.player2 = player2;

            player1.VideoRenderer = WPFMediaKit.DirectShow.MediaPlayers.VideoRendererType.VideoMixingRenderer9;
            player2.VideoRenderer = WPFMediaKit.DirectShow.MediaPlayers.VideoRendererType.VideoMixingRenderer9;

            player1.CurrentStateChanged += Player_CurrentStateChanged;
            player2.CurrentStateChanged += Player_CurrentStateChanged;

            loader1 = new MediaElementLoader(player1);
            loader2 = new MediaElementLoader(player2);
            
            StopLoad();
        }

        private void Player_CurrentStateChanged(MediaElementPro sender, PlayerState state)
        {
            if (state == PlayerState.Ended)
            {
                sender.Close();
            }
        }

        Queue<TrackItem> _allVideos = new Queue<TrackItem>();
        Queue<TrackItem> _loadWaitVideos = new Queue<TrackItem>();
        MediaElementPro player1, player2;
        MediaElementLoader loader1, loader2;

        public bool IsPlaying => 
            (player1?.IsPlaying).GetValueOrDefault() || (player2?.IsPlaying).GetValueOrDefault();

        public bool PlayerAssigned => (player1 != null) && (player2 != null);

        bool loading = false;

        #region [  TimeLine Event  ]

        private void TimeLine_ItemAdded(object sender, ItemEventArgs e)
        {
            _allVideos.Enqueue(e.Item);
        }

        
        int waitFrame => ((int)TimeLine.FrameRate.GetEnumAttribute<DefaultValueAttribute>().Value) * 5;

        private void TimeLine_FrameChanged(object sender, EventArgs e)
        {
            if (!PlayerAssigned)
                throw new NullReferenceException("player1 또는 player2가 할당되지 않았습니다. SetPlayer 함수를 이용해서 할당해주세요.");

            if (!TimeLine.IsRunning)
                return;

            if (loading)
            {
                LoadCheck();
                int position = TimeLine.Position;

                IEnumerable<TrackItem> readyItems = TimeLine.GetItems(position, position + waitFrame, Track, FindRangeType.FindStartPoint);
                readyItems.ForEach(i => ItemReady?.Invoke(i, new TimingReadyEventArgs(TimeLine, i.Offset, i.Offset - TimeLine.Position)));
            }
        }

        #endregion

        private void LoadWaitingVideos()
        {
            if (!TimeLine.IsRunning && !TimeLine.IsReady)
                return;

            if (_loadWaitVideos.Count == 0)
                return;

            if (!loader1.IsReadyForPlay)
            {
                loader1.LoadVideo(_loadWaitVideos.Dequeue()).Wait();
            }
            else if (!loader2.IsReadyForPlay)
            {
                loader2.LoadVideo(_loadWaitVideos.Dequeue()).Wait();
            }
        }

        bool _waitTask = false;

        /// <summary>
        /// 특정 상황에서 작업을 대기하게 합니다.
        /// </summary>
        public void WaitTask()
        {
            _waitTask = true;
        }

        /// <summary>
        /// 대기했던 작업을 완료했음을 알립니다.
        /// </summary>
        public void DoneTask()
        {
            _waitTask = false;    
        }

        public void StartLoad()
        {
            loading = true;
            Console.WriteLine("Start Load Start");
            Application.Current.Dispatcher.Invoke(() =>
            {
                IEnumerable<TrackItem> readyItems = TimeLine.GetItems(TimeLine.Position, TimeLine.Position + waitFrame, Track, FindRangeType.FindContains);
                readyItems.ForEach(i => ItemReady?.Invoke(i, new TimingReadyEventArgs(TimeLine, i.Offset, i.Offset - TimeLine.Position)));
                if (readyItems.Count() > 0)
                {
                    WaitTask();
                }
            });

            while (_waitTask)
            {
                Thread.Sleep(100);
            }

            Console.WriteLine("Start Load End");
        }
        
        public void StopLoad()
        {
            loading = false;

            if (player1.CurrentState == PlayerState.Playing)
            {
                DisablePlayer(player1);
            }
            
            if (player2.CurrentState == PlayerState.Playing)
            {
                DisablePlayer(player2);
            }
        }

        public void DisablePlayer(MediaElementPro player)
        {
            player.Stop();
            player.Close();
            player.Source = null;
            player.Volume = 0;
            player.Play();
            player.Visibility = Visibility.Hidden;
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
            return TimeLine.AllItems.Where(i => i.Offset <= frame && i.Offset + i.FrameWidth > frame);
        }
        
        private void LoadCheck()
        {
            if (_allVideos.Count == 0)
                return;

            TrackItem item = _allVideos.Peek();
            if ((item.Offset - TimeLine.Position) < MediaTools.TimeSpanToFrame(TimeSpan.FromSeconds(10), TimeLine.FrameRate))
            {
                DebugHelper.WriteLine("Should be Load!" + item.OriginalPath);

                _loadWaitVideos.Enqueue(item);

                _allVideos.Dequeue();
            }
        }
    }
}
