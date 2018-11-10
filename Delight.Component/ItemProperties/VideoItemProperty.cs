using Delight.Component.Common;
using Delight.Component.PropertyEditor;
using System;
using System.Activities.Presentation.PropertyEditing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Delight.Component.ItemProperties
{
    public class VideoItemProperty : BaseTrackItemProperty
    {

        Percentage _left = new Percentage(0, 0, 1);

        [Category("공통")]
        [DisplayName("왼쪽 위치")]
        [Description("현재 재생 중인 아이템의 왼쪽 위치를 지정합니다.")]
        [Editor(typeof(PercentageEditor), typeof(PropertyValueEditor))]
        public Percentage Left
        {
            get => _left;
            set { _left = value; PropChanged("Left"); }
        }

        Percentage _top = new Percentage(0, 0, 1);

        [Category("공통")]
        [DisplayName("위쪽 위치")]
        [Description("현재 재생 중인 아이템의 위쪽 위치를 지정합니다.")]
        [Editor(typeof(PercentageEditor), typeof(PropertyValueEditor))]

        public Percentage Top
        {
            get => _top;
            set { _top = value; PropChanged("Top"); }
        }

        Percentage _opacity = new Percentage(1, 0, 1);

        [Category("공통")]
        [DisplayName("투명도")]
        [Description("현재 재생 중인 아이템의 투명도를 설정합니다.")]
        [Editor(typeof(PercentageEditor), typeof(PropertyValueEditor))]
        public Percentage Opacity
        {
            get => _opacity;
            set { _opacity = value; PropChanged("Opacity"); }
        }

        Color _color = Colors.Black;

        [Editor(typeof(ColorEditor), typeof(PropertyValueEditor))]
        public Color Color
        {
            get => _color;
            set {
                _color = value;
                PropChanged("Color");
            }
        }

        private void Test()
        {
        }
    }
}
