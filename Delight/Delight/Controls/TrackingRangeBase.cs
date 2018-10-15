using Delight.Common;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;

namespace Delight.Controls
{
    [DefaultEvent(nameof(ValueChanged)), DefaultProperty(nameof(Value))]
    public class TrackingRangeBase : Control
    {
        #region Events
        public static readonly RoutedEvent ValueChangedEvent = 
            EventManager.RegisterRoutedEvent(
                nameof(ValueChanged),
                RoutingStrategy.Bubble, 
                typeof(RoutedPropertyChangedEventHandler<double>), 
                typeof(TrackingRangeBase));

        public static readonly RoutedEvent TrackValueChangedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(TrackValueChanged),
                RoutingStrategy.Bubble,
                typeof(TrackValueChangedEventHandler),
                typeof(TrackingRangeBase));

        public static readonly RoutedEvent TrackingStartedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(TrackingStarted),
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(TrackingRangeBase));

        public static readonly RoutedEvent TrackingStoppedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(TrackingStopped),
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(TrackingRangeBase));

        [Category("Behavior")]
        public event RoutedPropertyChangedEventHandler<double> ValueChanged
        {
            add => AddHandler(ValueChangedEvent, value);
            remove => RemoveHandler(ValueChangedEvent, value);
        }

        [Category("Behavior")]
        public event TrackValueChangedEventHandler TrackValueChanged
        {
            add => AddHandler(TrackValueChangedEvent, value);
            remove => RemoveHandler(TrackValueChangedEvent, value);
        }

        [Category("Behavior")]
        public event RoutedEventHandler TrackingStarted
        {
            add => AddHandler(TrackingStartedEvent, value);
            remove => RemoveHandler(TrackingStartedEvent, value);
        }

        [Category("Behavior")]
        public event RoutedEventHandler TrackingStopped
        {
            add => AddHandler(TrackingStoppedEvent, value);
            remove => RemoveHandler(TrackingStoppedEvent, value);
        }
        #endregion

        #region Properties
        public static readonly DependencyProperty MinimumProperty =
            DependencyHelper.Register(
                new FrameworkPropertyMetadata(0.0d, new PropertyChangedCallback(OnMinimumChanged)),
                new ValidateValueCallback(IsValidDoubleValue));

        public static readonly DependencyProperty MaximumProperty =
            DependencyHelper.Register(
                new FrameworkPropertyMetadata(1.0d, new PropertyChangedCallback(OnMaximumChanged),
                    new CoerceValueCallback(CoerceMaximum)),
                new ValidateValueCallback(IsValidDoubleValue));

        public static readonly DependencyProperty ValueProperty =
            DependencyHelper.Register(
                new FrameworkPropertyMetadata(
                    0.0d,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
                    new PropertyChangedCallback(OnValueChanged),
                    new CoerceValueCallback(ConstrainToRange)),
                new ValidateValueCallback(IsValidDoubleValue));

        private static readonly DependencyPropertyKey TrackValuePropertyKey =
            DependencyHelper.RegisterReadOnly(
                new FrameworkPropertyMetadata(
                    0.0d,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
                    new PropertyChangedCallback(OnTrackValueChanged),
                    new CoerceValueCallback(ConstrainToRange)),
                new ValidateValueCallback(IsValidDoubleValue));

        public static readonly DependencyProperty TrackValueProperty = TrackValuePropertyKey.DependencyProperty;

        public static readonly DependencyProperty LargeChangeProperty =
            DependencyHelper.Register(
                new FrameworkPropertyMetadata(1.0d),
                new ValidateValueCallback(IsValidChange));

        public static readonly DependencyProperty SmallChangeProperty =
            DependencyHelper.Register(
                new FrameworkPropertyMetadata(0.1d),
                new ValidateValueCallback(IsValidChange));

        [Bindable(true), Category("Behavior")]
        public double Minimum
        {
            get => (double)this.GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        [Bindable(true), Category("Behavior")]
        public double Maximum
        {
            get => (double)this.GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        [Bindable(true), Category("Behavior")]
        public double Value
        {
            get => (double)this.GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        [Bindable(true), Category("Behavior")]
        public double TrackValue
        {
            get => (double)this.GetValue(TrackValuePropertyKey.DependencyProperty);
        }

        [Bindable(true), Category("Behavior")]
        public double LargeChange
        {
            get => (double)this.GetValue(LargeChangeProperty);
            set => SetValue(LargeChangeProperty, value);
        }

        [Bindable(true), Category("Behavior")]
        public double SmallChange
        {
            get => (double)this.GetValue(SmallChangeProperty);
            set => SetValue(SmallChangeProperty, value);
        }
        #endregion

        #region Property Callbacks
        static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var range = (TrackingRangeBase)d;

            range.CoerceValue(MaximumProperty);
            range.CoerceValue(ValueProperty);
            range.OnMinimumChanged((double)e.OldValue, (double)e.NewValue);
        }

        static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var range = (TrackingRangeBase)d;
            
            range.CoerceValue(ValueProperty);
            range.OnMaximumChanged((double)e.OldValue, (double)e.NewValue);
        }

        static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var range = (TrackingRangeBase)d;

            range.OnValueChanged((double)e.OldValue, (double)e.NewValue);
        }

        static void OnTrackValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var range = (TrackingRangeBase)d;

            range.OnTrackValueChanged((double)e.OldValue, (double)e.NewValue);
        }

        static object ConstrainToRange(DependencyObject d, object value)
        {
            var range = (TrackingRangeBase)d;
            double min = range.Minimum;
            double v = (double)value;

            if (v < min)
                return min;

            double max = range.Maximum;

            if (v > max)
                return max;

            return value;
        }

        static object CoerceMaximum(DependencyObject d, object value)
        {
            var range = (TrackingRangeBase)d;
            double min = range.Minimum;

            if ((double)value < min)
            {
                return min;
            }

            return value;
        }

        static bool IsValidDoubleValue(object value)
        {
            double d = (double)value;

            return !(double.IsNaN(d) || double.IsInfinity(d));
        }

        static bool IsValidChange(object value)
        {
            double d = (double)value;

            return IsValidDoubleValue(value) && d >= 0.0;
        }
        #endregion

        #region Local Variable
        bool _isTracking;
        #endregion

        protected void BeginTracking()
        {
            if (_isTracking)
                return;

            _isTracking = true;

            RaiseEvent(new RoutedEventArgs(TrackingStartedEvent));
        }

        protected void EndTracking()
        {
            if (!_isTracking)
                return;

            SetCurrentValue(ValueProperty, TrackValue);

            _isTracking = false;

            RaiseEvent(new RoutedEventArgs(TrackingStoppedEvent));
        }

        protected void SetTrackValue(double value)
        {
            if (!_isTracking)
                throw new Exception();

            SetValue(TrackValuePropertyKey, value);
        }

        protected virtual void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
        }

        protected virtual void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
        }

        protected virtual void OnValueChanged(double oldValue, double newValue)
        {
            var args = new RoutedPropertyChangedEventArgs<double>(oldValue, newValue);
            args.RoutedEvent = ValueChangedEvent;
            RaiseEvent(args);

            if (!_isTracking)
                SetValue(TrackValuePropertyKey, newValue);
        }

        protected virtual void OnTrackValueChanged(double oldValue, double newValue)
        {
            var args = new TrackValueChangedEventArgs(oldValue, newValue, _isTracking);
            args.RoutedEvent = TrackValueChangedEvent;
            RaiseEvent(args);
        }
    }
}