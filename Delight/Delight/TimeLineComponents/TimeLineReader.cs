using Delight.Components.Common;
using Delight.Controls;
using Delight.LogManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private void _timeLine_ItemAdded(object sender, ItemEventArgs e)
        {
            if (e.Item.TrackType == TrackType.Video)
            {
                var item = e.Item;

                _loadWaitVideos.Enqueue(item);
                PrintLoadVideosInfo();
            }
        }

        public void SetPlayer(MediaElement player1, MediaElement player2)
        {
            if (player1 == player2)
                throw new Exception("player1와 player2는 같은 인스턴스 일 수 없습니다.");

            this.player1 = player1;
            this.player2 = player2;
        }

        public void PrintLoadVideosInfo()
        {
            Console.WriteLine("------------------------------------");
            Console.WriteLine("            Loaded Videos           ");
            Console.WriteLine("------------------------------------");

            int i = 1;
            foreach(TrackItem item in _loadWaitVideos)
            {
                Console.WriteLine(i++ + " Item");
                Console.WriteLine("Location : " + item.OriginalPath);
                Console.WriteLine("StartPosition : " + item.Offset);
                Console.WriteLine("------------------------------------");
            }
        }

        public Queue<TrackItem> _loadWaitVideos = new Queue<TrackItem>();

        MediaElement player1, player2;

        public bool PlayerAssigned => (player1 != null) && (player2 != null);

        bool loading = false;

        public void StartLoad()
        {
            loading = true;

            _loadWaitVideos.Clear();
            foreach (TrackItem item in _timeLine.GetItems(TrackType.Video, _timeLine.Position).OrderBy(i => i.Offset))
            {
                _loadWaitVideos.Enqueue(item);
            }
        }

        public void StopLoad()
        {
            loading = false;
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
                if (_loadWaitVideos.Count == 0)
                    return;
                TrackItem item = _loadWaitVideos.Peek();
                if ((item.Offset - _timeLine.Position) < MediaTools.TimeSpanToFrame(TimeSpan.FromSeconds(10), _timeLine.FrameRate))
                {
                    Console.WriteLine("Should be Load!" + item.OriginalPath);

                    _loadWaitVideos.Dequeue();
                }
            }
        }

        TimeLine _timeLine;
    }
}
