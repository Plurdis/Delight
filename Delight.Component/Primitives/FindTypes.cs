using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Component.Primitives
{
    public enum FindType
    {
        /// <summary>
        /// 오직 시작 지점을 찾습니다.
        /// </summary>
        FindStartPoint = 0,
        /// <summary>
        /// 오직 종료 지점을 찾습니다.
        /// </summary>
        FindEndPoint = 1,
        /// <summary>
        /// 포함되어 있는 지점을 찾습니다.
        /// </summary>
        FindContains = 2,
        /// <summary>
        /// 시작 지점을 제외하고 포함되어 있는 지점을 찾습니다.
        /// </summary>
        FindWithoutStartPoint = 3,
        /// <summary>
        /// 종료 지점을 제외하고 포함되어 있는 지점을 찾습니다.
        /// </summary>
        FindWithoutEndPoint = 4,
        /// <summary>
        /// 시작과 종료 지점을 제외하고 포함되어 있는 지점을 찾습니다.
        /// </summary>
        FindWithoutStartEndPoint = 5,
        /// <summary>
        /// 해당 프레임 이후에 포함되어 있는 아이템들을 찾습니다. (현재 프레임 포함)
        /// </summary>
        FindAfterFrame = 6,
        /// <summary>
        /// 해당 프레임 이후에 포함되어 있는 아이템들을 찾습니다. (현재 프레임 제외)
        /// </summary>
        FindAfterFrameExceptThis = 7,
        /// <summary>
        /// 해당 프레임 이전의 아이템들을 찾습니다. (현재 프레임 포함)
        /// </summary>
        FindBeforeFrame = 8,
        /// <summary>
        /// 해당 프레임 이전의 아이템들을 찾습니다. (현재 프레임 제외)
        /// </summary>
        FindBeforeFrameExceptThis = 9,
    }

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
