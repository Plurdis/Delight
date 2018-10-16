using Delight.Controls;
using Delight.Core.Common;
using Delight.Core.Extensions;
using Delight.Timing.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Timing
{
    public class TimingReader
    {
        /// <summary>
        /// 현재 TrackItem이 재생 종료되었음을 나타냅니다.
        /// </summary>
        public event TimingDelegate ItemEnded;

        internal void OnItemEnded(TrackItem sender, TimingEventArgs e)
        {
            ItemEnded?.Invoke(sender, e);
            Console.WriteLine($"Item Ended => {sender.Text}");
        }

        /// <summary>
        /// 현재 TrackItem이 재생중임을 나타냅니다.
        /// </summary>
        public event TimingDelegate ItemPlaying;

        internal void OnItemPlaying(TrackItem sender, TimingEventArgs e)
        {
            ItemPlaying?.Invoke(sender, e);
            //Console.WriteLine($"Item Playing => {sender.Text}");
        }

        public event TimingDelegate ItemStarted;

        internal void OnItemStarted(TrackItem sender, TimingEventArgs e)
        {
            ItemStarted?.Invoke(sender, e);
            Console.WriteLine($"Item Started => {sender.Text}");
        }

        public event EventHandler TimeLineStarted;

        internal void OnTimeLineStarted(object sender, EventArgs e)
        {
            TimeLineStarted?.Invoke(sender, e);
        }

        public event EventHandler TimeLineStopped;

        internal void OnTimeLineStopped(object sender, EventArgs e)
        {
            TimeLineStopped?.Invoke(sender, e);
        }

        #region [  Properties  ]

        internal TimeLine TimeLine { get; }
        internal Track Track { get; }

        public int CurrentFrame => TimeLine.Position;
        public FrameRate CurrentFrameRate => TimeLine.FrameRate;

        #endregion

        /// <summary>
        /// 선택된 TimeLine의 Track을 한정으로 재생 정보를 받는 <see cref="TimingReader"/>를 토기화합니다.
        /// </summary>
        /// <param name="timeLine"></param>
        /// <param name="track"></param>
        public TimingReader(TimeLine timeLine, Track track)
        {
            TimeLine = timeLine;
            Track = track;

            TimeLine.FrameChanged += TimeLine_FrameChanged;
            TimeLine.FrameMouseChanged += (s, e) =>
            {
                lastPlayingItem.ForEach(i => OnItemEnded(i, new TimingEventArgs(timeLine, CurrentFrame)));
                lastPlayingItem = Enumerable.Empty<TrackItem>();
            };

            TimeLine.TimeLineStarted += (s, e) => TimeLineStarted?.Invoke(s, e);
            TimeLine.TimeLineStopped += (s, e) =>
            {
                lastPlayingItem.ForEach(i => OnItemEnded(i, new TimingEventArgs(timeLine, CurrentFrame)));
                lastPlayingItem = Enumerable.Empty<TrackItem>();
                TimeLineStopped?.Invoke(s, e);
            };
        }

        IEnumerable<TrackItem> lastPlayingItem = Enumerable.Empty<TrackItem>();

        private void TimeLine_FrameChanged(object sender, EventArgs e)
        {
            if (!TimeLine.IsRunning)
                return;

            int position = TimeLine.Position;

            IEnumerable<TrackItem> playingItems = TimeLine.GetItems(position - 1, Track, FindType.FindContains);
            playingItems.ForEach(i => OnItemPlaying(i, new TimingEventArgs(TimeLine, TimeLine.Position)));

            //if (lastPlayingItem == null)
            //    lastPlayingItem = Enumerable.Empty<TrackItem>();

            playingItems.Except(lastPlayingItem).ForEach(i => 
                OnItemStarted(i, new TimingEventArgs(TimeLine, TimeLine.Position)));

            // 가장 마지막에 플레이로 인식된 아이템

            IEnumerable<TrackItem> enditems = TimeLine.GetItems(position, Track, FindType.FindEndPoint);
            enditems.Concat(lastPlayingItem.Except(playingItems)).ForEach(i => OnItemEnded(i, new TimingEventArgs(TimeLine, TimeLine.Position)));

            lastPlayingItem = new List<TrackItem>(playingItems);

        }
    }
}
