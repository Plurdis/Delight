using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Delight.Timing
{
    public class TrackItemProperty
    {
        public TrackItemProperty()
        {
        }

        #region [  Opacity  ]

        double _opacity = 1.0;
        public double Opacity
        {
            get => _opacity;
            set
            {
                _opacity = value;
                _propertyChanged?.Invoke(this, new PropertyChangedEventArgs("opacity"));
            }
        }

        public double CurrentOpacity { get; private set; }

        #endregion

        #region [  Size  ]

        double _size = 1.0;
        public double Size
        {
            get => _size;
            set
            {
                _size = value;
                _propertyChanged?.Invoke(this, new PropertyChangedEventArgs("size"));
            }
        }

        public double CurrentSize { get; private set; }

        #endregion

        #region [  Volume  ]

        double _volume = 1.0;

        public double Volume
        {
            get => _volume;
            set
            {
                _volume = value;
                _propertyChanged?.Invoke(this, new PropertyChangedEventArgs("volume"));
            }
        }

        public double CurrentVolume { get; private set; }

        #endregion

        #region [  PositionX  ]

        double _positionX;

        public double PositionX
        {
            get => _positionX;
            set
            {
                _positionX = value;
                _propertyChanged?.Invoke(this, new PropertyChangedEventArgs("positionX"));
            }
        }

        #endregion

        #region [  PositionY  ]

        double _positionY;

        public double PositionY
        {
            get => _positionY;
            set
            {
                _positionY = value;
                _propertyChanged?.Invoke(this, new PropertyChangedEventArgs("positionY"));
            }
        }

        #endregion

        #region [  ChromaKey  ]

        bool _chromaKeyEnabled = false;

        public bool ChromaKeyEnabled
        {
            get => _chromaKeyEnabled;
            set
            {
                _chromaKeyEnabled = value;
                _propertyChanged?.Invoke(this, new PropertyChangedEventArgs(("chromaKeyEnabled")));
            }
        }

        Color _chromaKeyColor = Color.FromArgb(255,0,0,0);

        public Color ChromaKeyColor
        {
            get => _chromaKeyColor;
            set
            {
                _chromaKeyColor = value;
                _propertyChanged?.Invoke(this, new PropertyChangedEventArgs("ChromaKeyColor"));
            }
        }

        double _chromaKeyUsage = 0.2;

        public double ChromaKeyUsage
        {
            get => _chromaKeyUsage;
            set
            {
                _chromaKeyUsage = value;
                _propertyChanged?.Invoke(this, new PropertyChangedEventArgs("ChromaKeyUsage"));
            }
        }

        #endregion

        public event EventHandler<PropertyChangedEventArgs> PropertyChanged
        {
            add
            {
                propChangedHandlers.Add(value);
                _propertyChanged += value;
            }

            remove
            {
                propChangedHandlers.Remove(value);
                _propertyChanged -= value;
            }
        }

        private event EventHandler<PropertyChangedEventArgs> _propertyChanged;

        private List<EventHandler<PropertyChangedEventArgs>> propChangedHandlers = new List<EventHandler<PropertyChangedEventArgs>>();

        public void ResetEventHandler()
        {
            propChangedHandlers.ForEach(i => _propertyChanged -= i);
            propChangedHandlers.Clear();
        }

    }
}
