using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Timing.Common
{
    public enum FindRangeType
    {
        /// <summary>
        /// StartFrame과 EndFrame이 정확히 일치한 지점을 찾습니다.
        /// </summary>
        FindExcatly,
        /// <summary>
        /// 지정된 Frame 범위에 시작점이 겹치는 지점을 찾습니다.
        /// </summary>
        FindStartPoint,
        /// <summary>
        /// 지정된 Frame 범위에 종료점이 겹치는 지점을 찾습니다.
        /// </summary>
        FindEndPoint,
        /// <summary>
        /// 지정된 Frame 범위에 시작점과 종료점이 포함되어 있는 지점을 찾습니다.
        /// </summary>
        FindContains,

    }
}
