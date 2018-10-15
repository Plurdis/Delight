using Delight.Controls;
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

        /// <summary>
        /// 현재 TrackItem이 재생중임을 나타냅니다.
        /// </summary>
        public event TimingDelegate ItemPlaying;

        public event EventHandler TimeLineStarted;
        public event EventHandler TimeLineStopped;

        #region [  Properties  ]

        TimeLine TimeLine { get; }
        Track Track { get; }

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
            //TimeLine.FrameMouseChanged += (s, e) => StopLoad();

            TimeLine.TimeLineStarted += (s, e) => TimeLineStarted?.Invoke(s, e);
            TimeLine.TimeLineStopped += (s, e) => TimeLineStopped?.Invoke(s, e);

            TimeLine.ItemAdded += TimeLine_ItemAdded;
        }

        private void TimeLine_FrameChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
