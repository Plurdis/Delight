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
    public abstract class BaseTrackItemProperty : INotifyPropertyChanged
    {
        bool _isEnabled = true;

        [Category("공통")]
        [DisplayName("사용")]
        public bool IsEnabled
        {
            get => _isEnabled;
            set { _isEnabled = value; PropChanged("IsEnabled"); }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void PropChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
