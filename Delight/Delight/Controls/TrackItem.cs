using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Delight.Common;
using Delight.Controls;

namespace Delight.Controls
{
    /// <summary>
    /// <see cref="TimeLine"/> 트랙에서 움직이는 컨트롤을 나타냅니다.
    /// </summary>
    /// <remarks>Represents a control that move in <see cref="TimeLine"/> Tracks.</remarks>
    public class TrackItem : Control
    {
        public TrackItem()
        {
            this.Style = FindResource("TrackItemStyle") as Style;
        }


        public int Offset { get; set; }

        public int ValueWidth { get; set; }

        public static DependencyProperty ColorThemeProperty = DependencyProperty.Register(nameof(ColorTheme), typeof(ColorTheme), typeof(TemplateItem));

        public ColorTheme ColorTheme
        {
            get => (ColorTheme)GetValue(ColorThemeProperty);
            set => SetValue(ColorThemeProperty, value);
        }
    }
}
