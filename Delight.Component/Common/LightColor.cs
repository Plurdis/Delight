using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Delight.Component.Common
{
    public class LightColor
    {
        public LightColor()
        {

        }

        public LightColor(string colorName, SolidColorBrush color)
        {
            ColorName = colorName;
            FirstColor = color;
        }

        public LightColor(string colorName, SolidColorBrush firstColor, SolidColorBrush secondColor)
        {
            ColorName = colorName;
            FirstColor = firstColor;
            SecondColor = secondColor;
        }

        public SolidColorBrush FirstColor { get; set; } = Brushes.Transparent;
        
        public SolidColorBrush SecondColor { get; set; } = Brushes.Transparent;

        public string ColorName { get; set; }
    }
}
