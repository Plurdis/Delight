using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Delight.Common;

namespace Delight.Controls
{
    /// <summary>
    /// <see cref="TimeLine"/> 트랙에서 움직이는 컨트롤을 나타냅니다.
    /// </summary>
    /// <remarks>Represents a control that move in <see cref="TimeLine"/> Tracks.</remarks>
    [TemplatePart(Name = "dragLeft", Type = typeof(Rectangle))]
    [TemplatePart(Name = "dragMove", Type = typeof(Rectangle))]
    [TemplatePart(Name = "dragRight", Type = typeof(Rectangle))]
    public class TrackItem : Control
    {
        public TrackItem()
        {
            this.Style = FindResource("TrackItemStyle") as Style;
        }

        public event MouseButtonEventHandler LeftSide_MouseLeftButtonDown;
        public event MouseButtonEventHandler RightSide_MouseLeftButtonDown;
        public event MouseButtonEventHandler MovingSide_MouseLeftButtonDown;
        


        Rectangle leftSide, movingSide, rightSide;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            leftSide = GetTemplateChild("dragLeft") as Rectangle;
            movingSide = GetTemplateChild("dragMove") as Rectangle;
            rightSide = GetTemplateChild("dragRight") as Rectangle;

            leftSide.MouseLeftButtonDown += (s,e) => LeftSide_MouseLeftButtonDown?.Invoke(s,e);
            rightSide.MouseLeftButtonDown += (s,e) => RightSide_MouseLeftButtonDown?.Invoke(s,e);
            movingSide.MouseLeftButtonDown += (s,e) => MovingSide_MouseLeftButtonDown?.Invoke(s,e);
        }

        public int Offset { get; set; }

        /// <summary>
        /// 앞쪽에서 시작할 오프셋을 가져오거나 설정합니다.
        /// </summary>
        public int ForwardOffset { get; set; }

        /// <summary>
        /// 앞쪽에서 끝낼 오프셋을 가져오거나 설정합니다.
        /// </summary>
        public int BackwardOffset { get; set; }
        
        /// <summary>
        /// TrackItem에서 할당 가능한 최대 프레임을 가져오거나 설정합니다.
        /// </summary>
        public int MaxFrame { get; set; }

        /// <summary>
        /// 프레임 단위에서의 길이를 나타냅니다.
        /// </summary>
        public int FrameWidth { get; set; }
        
        public static DependencyProperty ColorThemeProperty = DependencyProperty.Register(nameof(ColorTheme), typeof(ColorTheme), typeof(TrackItem));

        public ColorTheme ColorTheme
        {
            get => (ColorTheme)GetValue(ColorThemeProperty);
            set => SetValue(ColorThemeProperty, value);
        }

        public static DependencyProperty ThumbnailProperty = DependencyProperty.Register(nameof(Thumbnail), typeof(ImageSource), typeof(TrackItem));

        public ImageSource Thumbnail
        {
            get => GetValue(ThumbnailProperty) as ImageSource;
            set => SetValue(ThumbnailProperty, value);
        }

        public static DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(TrackItem));

        public string Text
        {
            get => GetValue(TextProperty) as string;
            set => SetValue(TextProperty, value);
        }
    }
}
