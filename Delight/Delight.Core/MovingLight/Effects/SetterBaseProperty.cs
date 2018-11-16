using Delight.Component.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight
{
    public class BaseLightSetterProperty : INotifyPropertyChanged
    {
        [DesignElement(DisplayName = "재생 속도 비율", Key = "Speed", Category = "공통")]
        public double Speed { get; set; } = 1;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}