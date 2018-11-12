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

        bool _chromaKeyUse = false;

        [Category("크로마키")]
        [DisplayName("사용")]
        [Description("크로마키 효과 사용 여부를 설정합니다.")]
        public bool ChromaKeyUse
        {
            get => _chromaKeyUse;
            set { _chromaKeyUse = value; PropChanged("ChromaKeyUse"); }
        }

        ColorData _chromaKeyColor = new ColorData(Colors.Green);

        [Category("크로마키")]
        [DisplayName("색상")]
        [Description("크로마키 할 색상을 설정합니다.")]
        [Editor(typeof(ColorEditor), typeof(PropertyValueEditor))]
        public ColorData ChromaKeyColor
        {
            get => _chromaKeyColor;
            set { _chromaKeyColor = value; PropChanged("ChromaKeyColor"); }
        }
        
        Percentage _chromaKeyUsage = new Percentage(0.4, 0, 1);

        [Category("크로마키")]
        [DisplayName("사용도")]
        [Description("크로마키 사용도를 설정합니다.")]
        [Editor(typeof(PercentageEditor), typeof(PropertyValueEditor))]
        public Percentage ChromaKeyUsage
        {
            get => _chromaKeyUsage;
            set { _chromaKeyUsage = value; PropChanged("ChromaKeyUsage"); }
        }



        private void Test()
        {
        }
    }
}
