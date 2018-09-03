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
        [Description("이미지")]
        Image = 1,
        [Description("비디오")]
        Video = 2,
        [Description("시각 효과")]
        Unity = 4,
        [Description("사운드")]
        Sound = 8,
        [Description("시각 효과")]
        Effect = 16,
        [Description("템플릿")]
        Template = 32,
        [Description("조명")]
        Light = 64,
        Unknown = int.MaxValue,
    }
}
