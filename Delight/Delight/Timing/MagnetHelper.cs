using Delight.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        //public int GetMagnetValue(TrackItem item, int value, DragSide dragSide)
        //{
        //    var items = track.Items.Where(i => i != item);
        //    switch (dragSide)
        //    {
        //        case DragSide.LeftSide:

        //            break;
        //        case DragSide.RightSide:
        //            break;
        //        case DragSide.MovingSide:
        //            break;
        //        case DragSide.Unknown:
        //        default:
        //            break;
        //    }
        //}

        //private int GetSimilarValue(double[] values, double targetValue, double recognizeRange )
        //{

        //}
    }
}
