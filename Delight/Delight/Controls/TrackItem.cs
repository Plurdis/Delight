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

        public event MouseButtonEventHandler DragLeftMouseLeftButtonDown;
        public event MouseButtonEventHandler DragRightMouseLeftButtonDown;
        public event MouseButtonEventHandler DragMoveMouseLeftButtonDown;

        Rectangle dragLeft, dragMove, dragRight;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            dragLeft = GetTemplateChild("dragLeft") as Rectangle;
            dragMove = GetTemplateChild("dragMove") as Rectangle;
            dragRight = GetTemplateChild("dragRight") as Rectangle;

            dragLeft.MouseLeftButtonDown += (s,e) => DragLeftMouseLeftButtonDown?.Invoke(s,e);
            dragRight.MouseLeftButtonDown += (s,e) => DragRightMouseLeftButtonDown?.Invoke(s,e);
            dragMove.MouseLeftButtonDown += (s,e) => DragMoveMouseLeftButtonDown?.Invoke(s,e);
        }

        public int Offset { get; set; }

        public int ValueWidth { get; set; }

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
