using Delight.Component.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Delight.Component.Layers
{
    public class BaseLayer : Control, ILayer
    {
        public BaseLayer(Track track)
        {
            this.Track = track;
        }

        public Track Track { get; }
    }
}
