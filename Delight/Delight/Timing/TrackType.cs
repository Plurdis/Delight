using Delight.Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Timing
{
    public enum TrackType
    {
        [PlayDevice(PlayDevice.Display)]
        [Description("이미지")]
        Image = 1,

        [PlayDevice(PlayDevice.Display)]
        [Description("비디오")]
        Video = 2,

        [PlayDevice(PlayDevice.Display)]
        [Description("효과 분류")]
        Unity = 4,

        [PlayDevice(PlayDevice.Speaker)]
        [Description("사운드")]
        Sound = 8,

        [Description("시각 효과")]
        Effect = 16,

        [Description("템플릿")]
        Template = 32,

        [PlayDevice(PlayDevice.Light)]
        [Description("조명")]
        Light = 64,

        Unknown = int.MaxValue,
    }
}
