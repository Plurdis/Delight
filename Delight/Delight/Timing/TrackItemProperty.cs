using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Timing
{
    public class TrackItemProperty
    {
        public TrackItemProperty()
        {
            _opacity = 1.0;
        }

        #region [  Opacity  ]

        double _opacity;
        public double Opacity
        {
            get => _opacity;
            set
            {
                _opacity = value;
                _opacityChanged?.Invoke(this, new EventArgs());
            }
        }

        public double CurrentOpacity { get; private set; }


        private event EventHandler _opacityChanged;

        private List<EventHandler> opacityHandlers = new List<EventHandler>();

        public event EventHandler OpacityChanged
        {
            add
            {
                opacityHandlers.Add(value);
                _opacityChanged += value;
            }
            remove
            {
                opacityHandlers.Remove(value);
                _opacityChanged -= value;
            }
        }

        public void ResetOpacityHandler()
        {
            opacityHandlers.ForEach(i => _opacityChanged -= i);
            opacityHandlers.Clear();
        }

        #endregion

        #region [  Size  ]

        double _size;
        public double Size
        {
            get => _size;
            set
            {
                _size = value;
                _sizeChanged?.Invoke(this, new EventArgs());
            }
        }

        public double CurrentSize { get; private set; }


        private event EventHandler _sizeChanged;

        private List<EventHandler> sizeHandlers = new List<EventHandler>();

        public event EventHandler SizeChanged
        {
            add
            {
                sizeHandlers.Add(value);
                _sizeChanged += value;
            }
            remove
            {
                sizeHandlers.Remove(value);
                _sizeChanged -= value;
            }
        }

        public void ResetSizeHandler()
        {
            sizeHandlers.ForEach(i => _sizeChanged -= i);
            sizeHandlers.Clear();
        }

        #endregion
    }
}
