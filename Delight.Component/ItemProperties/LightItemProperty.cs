using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Component.ItemProperties
{
    public class LightItemProperty : BaseTrackItemProperty
    {

        int _speed;

        [Category("공통")]
        [DisplayName("속도")]
        [Description("현재 재생 중인 아이템의 효과를 지정합니다.")]
        public int Speed
        {
            get => _speed;
            set { _speed = value; PropChanged("Speed"); }
        }

        byte _lightColor;

        
    }
}
