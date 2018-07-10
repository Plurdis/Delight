using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StagePainter.Core.Common
{
    /// <summary>
    /// 프레임 레이트를 나타냅니다. <see cref="DefaultValueAttribute"/>를 통해서 값을 int 값을 추출할 수 있습니다.
    /// </summary>
    public enum FrameRate
    {
        /// <summary>
        /// 24 프레임을 나타냅니다.
        /// </summary>
        [DefaultValue(24)]
        _24FPS,
        /// <summary>
        /// 30 프레임을 나타냅니다.
        /// </summary>
        [DefaultValue(30)]
        _30FPS,
        /// <summary>
        /// 60 프레임을 나타냅니다.
        /// </summary>
        [DefaultValue(60)]
        _60FPS,
    }
}
