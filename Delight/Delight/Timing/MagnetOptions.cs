using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Timing
{
    public struct MagnetOptions
    {
        /// <summary>
        /// 현재 계산된 프레임 값을 나타냅니다. (반영된 값이 아닙니다.)
        /// </summary>
        public int CurrentValue { get; set; }

        /// <summary>
        /// 프레임 단위가 아닌 실제 보여질 때의 계산된 값을 나타냅니다. (반영된 값이 아닙니다.)
        /// </summary>
        public double RawCurrentValue { get; set; }

        /// <summary>
        /// 검색할 TrackItem의 최대 왼쪽 Margin 값을 계산합니다.
        /// </summary>
        public double MaxLeftMargin { get; set; }
    }
}
