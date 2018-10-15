using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Delight.Common
{
    public class TrackValueChangedEventArgs : RoutedEventArgs
    {
        public double OldValue { get; }

        public double NewValue { get; }

        public bool IsTracking { get; }

        public TrackValueChangedEventArgs(double oldValue, double newValue, bool tracking) : base()
        {
            OldValue = oldValue;
            NewValue = newValue;
            IsTracking = tracking;
        }

        public TrackValueChangedEventArgs(double oldValue, double newValue, bool isTracking, RoutedEvent routedEvent) : this(oldValue, newValue, isTracking)
        {
            RoutedEvent = routedEvent;
        }
    }


    public delegate void TrackValueChangedEventHandler(object sender, TrackValueChangedEventArgs e);
}
