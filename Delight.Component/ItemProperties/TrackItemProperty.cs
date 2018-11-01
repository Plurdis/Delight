using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Component.ItemProperties
{
    /// <summary>
    /// TrackItem에 적용되는 Property입니다.
    /// </summary>
    public abstract class BaseTrackItemProperty
    {
        [Category("공통")]
        [DisplayName("사용")]
        public bool IsEnabled { get; set; } = true;
    }
}
