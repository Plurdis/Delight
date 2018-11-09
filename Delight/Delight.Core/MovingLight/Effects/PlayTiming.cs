using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Core.MovingLight.Effects
{
    public enum PlayTiming
    {
        /// <summary>
        /// 이전 효과와 함께 재생합니다.
        /// </summary>
        PlayWithPrevEffect,
        /// <summary>
        /// 이전 효과 이후에 재생합니다.
        /// </summary>
        PlayAfterPrevEffect,
    }
}
