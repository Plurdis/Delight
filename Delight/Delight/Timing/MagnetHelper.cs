using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Delight.Controls;
using Delight.Core.Extensions;

namespace Delight.Timing
{
    public class MagnetHelper
    {
        public MagnetHelper(Track track, int responsiveness)
        {
            this._track = track;
            Responsiveness = responsiveness;
        }

        public int Responsiveness { get; }

        Track _track;

        public IEnumerable<TrackItem> Items => _track.Items;

        public bool DoLeftMagnet(TrackItem item, MagnetOptions options, out int value)
        {
            value = 0;
            if ((options.RawCurrentValue - Responsiveness) <= options.MaxLeftMargin)
            {
                IEnumerable<int> points = GetMagnetPoints(Items.Except(item));

                value = GetSimilarValue(points, options.CurrentValue);
                return true;
            }

            return false;
        }

        public IEnumerable<int> GetMagnetPoints(IEnumerable<TrackItem> items)
        {
            IEnumerable<int> offsets = items.Select(i => i.Offset);
            IEnumerable<int> offWidths = items.Select(i => i.Offset + i.FrameWidth);

            return offsets.Concat(offWidths);
        }

        public int GetSimilarValue(IEnumerable<int> values, int value)
        {
            int similarValue = int.MaxValue;

            values.ForEach(i =>
            {
                int space = Math.Abs(i - value);
                if (space <= 10 && space < similarValue)
                {
                    similarValue = space;
                }
            });

            return similarValue;
        }

        //public int GetMagnetValue(TrackItem item, int value, DragSide dragSide)
        //{
        //    var items = _track.Items.Where(i => i != item);
        //    switch (dragSide)
        //    {
        //        case DragSide.LeftSide:
        //            return GetLeftMagnet(item, value, items);
        //        case DragSide.RightSide:
        //            break;
        //        case DragSide.MovingSide:
        //            break;
        //        case DragSide.Unknown:
        //        default:
        //            break;
        //    }
        //}

        //public int GetLeftMagnet(TrackItem item, int value, IEnumerable<TrackItem> items)
        //{

        //}



        //private int GetSimilarValue(double[] values, double targetValue, double recognizeRange )
        //{

        //}
    }
}
