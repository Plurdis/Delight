using Delight.Component.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Component
{
    public class ItemEventArgs : EventArgs
    {
        public ItemEventArgs(TrackItem item)
        {
            this.Item = item;
        }

        public TrackItem Item { get; set; }
    }
}
