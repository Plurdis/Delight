using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StagePainter.Core.Common
{
    public enum FrameRate
    {
        [DefaultValue(24)]
        _24FPS,
        [DefaultValue(30)]
        _30FPS,
        [DefaultValue(60)]
        _60FPS,
    }
}
