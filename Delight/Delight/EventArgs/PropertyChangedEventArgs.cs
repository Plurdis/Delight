using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Timing
{
    public class PropertyChangedEventArgs : EventArgs
    {
        public PropertyChangedEventArgs(string changedProperty)
        {
            ChangedProperty = changedProperty;
        }

        public string ChangedProperty { get; }
    }
}
