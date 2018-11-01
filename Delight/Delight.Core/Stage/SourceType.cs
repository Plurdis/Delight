using Delight.Core.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Core.Stage
{
    [Flags]
    public enum SourceType
    {
        [OutputDevice(OutputDevice.Display)]
        [Description("이미지")]
        [IsVisualTrack(true)]
        Image = 1,

        [OutputDevice(OutputDevice.Display)]
        [Description("비디오")]
        [IsVisualTrack(true)]
        Video = 2,

        [Obsolete]
        [OutputDevice(OutputDevice.Display)]
        [Description("효과 분류")]
        [IsVisualTrack(false)]
        Unity = 4,

        [OutputDevice(OutputDevice.Speaker)]
        [Description("사운드")]
        [IsVisualTrack(false)]
        Sound = 8,

        [Obsolete]
        [Description("시각 효과")]
        [IsVisualTrack(false)]
        Effect = 16,

        [Obsolete]
        [Description("템플릿")]
        [IsVisualTrack(false)]
        Template = 32,

        
        [OutputDevice(OutputDevice.MovingLight)]
        [Description("조명")]
        [IsVisualTrack(false)]
        Light = 64,

        All = 127,

        Unknown = int.MaxValue,
    }
}
