using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Delight.Component.Common
{
    public class ColorData : INotifyPropertyChanged
    {
        public ColorData()
        {

        }

        public ColorData(Color color)
        {
            Color = color;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private Color _color = default(Color);

        public Color Color
        {
            get => _color;
            set
            {
                _color = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Color"));
            }
        }
    }
}
