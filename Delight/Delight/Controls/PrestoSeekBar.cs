using Delight.Common;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

using WPFTrack = System.Windows.Controls.Primitives.Track;

namespace Delight.Controls
{
    [DefaultEvent(nameof(ValueChanged)), DefaultProperty(nameof(Value))]
    [TemplatePart(Name = TrackName, Type = typeof(Track))]
    public class PrestoSeekBar : TrackingRangeBase
    {
        private const string TrackName = "PART_Track";

        public static RoutedCommand IncreaseLarge { get; }

        public static RoutedCommand IncreaseSmall { get; }

        public static RoutedCommand DecreaseLarge { get; }

        public static RoutedCommand DecreaseSmall { get; }

        public static RoutedCommand MinimizeValue { get; }

        public static RoutedCommand MaximizeValue { get; }

        public static readonly DependencyProperty IsTrackingProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty OrientationProperty =
            DependencyHelper.Register(
                new PropertyMetadata(Orientation.Horizontal),
                new ValidateValueCallback(IsValidOrientation));

        public static readonly DependencyProperty IsDirectionReversedProperty =
            DependencyHelper.Register(
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        public bool IsTracking
        {
            get => (bool)this.GetValue(IsTrackingProperty);
            set => SetValue(IsTrackingProperty, value);
        }

        public Orientation Orientation
        {
            get => (Orientation)this.GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        [Bindable(true), Category("Appearance")]
        public bool IsDirectionReversed
        {
            get => (bool)this.GetValue(IsDirectionReversedProperty);
            set => SetValue(IsDirectionReversedProperty, value);
        }

        WPFTrack _track;
        bool _isDragging;

        static PrestoSeekBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PrestoSeekBar), new FrameworkPropertyMetadata(typeof(PrestoSeekBar)));

            // Routed Command Initialize
            IncreaseLarge = new RoutedCommand(nameof(IncreaseLarge), typeof(PrestoSeekBar));
            DecreaseLarge = new RoutedCommand(nameof(DecreaseLarge), typeof(PrestoSeekBar));
            IncreaseSmall = new RoutedCommand(nameof(IncreaseSmall), typeof(PrestoSeekBar));
            DecreaseSmall = new RoutedCommand(nameof(DecreaseSmall), typeof(PrestoSeekBar));
            MinimizeValue = new RoutedCommand(nameof(MinimizeValue), typeof(PrestoSeekBar));
            MaximizeValue = new RoutedCommand(nameof(MaximizeValue), typeof(PrestoSeekBar));

            RegisterClassCommandBinding(IncreaseLarge, OnIncreaseLargeCommand, new SeekBarGesture(Key.PageUp, Key.PageDown, false));
            RegisterClassCommandBinding(DecreaseLarge, OnDecreaseLargeCommand, new SeekBarGesture(Key.PageDown, Key.PageUp, false));
            RegisterClassCommandBinding(IncreaseSmall, OnIncreaseSmallCommand, new SeekBarGesture(Key.Up, Key.Down, false), new SeekBarGesture(Key.Right, Key.Left, true));
            RegisterClassCommandBinding(DecreaseSmall, OnDecreaseSmallCommand, new SeekBarGesture(Key.Down, Key.Up, false), new SeekBarGesture(Key.Left, Key.Right, true));
            RegisterClassCommandBinding(MaximizeValue, OnMinimizeValueCommand, new KeyGesture(Key.Home));
            RegisterClassCommandBinding(MinimizeValue, OnMaximizeValueCommand, new KeyGesture(Key.End));

            // Metadata Override
            MinimumProperty.OverrideMetadata(typeof(PrestoSeekBar), new FrameworkPropertyMetadata(0.0d, FrameworkPropertyMetadataOptions.AffectsMeasure));
            MaximumProperty.OverrideMetadata(typeof(PrestoSeekBar), new FrameworkPropertyMetadata(10.0d, FrameworkPropertyMetadataOptions.AffectsMeasure));
            ValueProperty.OverrideMetadata(typeof(PrestoSeekBar), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure));

            // Thumb
            EventManager.RegisterClassHandler(typeof(PrestoSeekBar), Thumb.DragStartedEvent, (DragStartedEventHandler)OnThumbDragStarted);
            EventManager.RegisterClassHandler(typeof(PrestoSeekBar), Thumb.DragDeltaEvent, (DragDeltaEventHandler)OnThumbDragDelta);
            EventManager.RegisterClassHandler(typeof(PrestoSeekBar), Thumb.DragCompletedEvent, (DragCompletedEventHandler)OnThumbDragCompleted);

            // Focusing
            EventManager.RegisterClassHandler(typeof(PrestoSeekBar), Mouse.MouseDownEvent, (MouseButtonEventHandler)OnMouseLeftButtonDown, true);
        }

        #region Routed Commands Binding
        static void RegisterClassCommandBinding(RoutedCommand command, ExecutedRoutedEventHandler eventHandler, params InputGesture[] gestures)
        {
            CommandManager.RegisterClassCommandBinding(typeof(PrestoSeekBar), new CommandBinding(command, eventHandler));

            if (gestures != null)
            {
                for (int i = 0; i < gestures.Length; i++)
                    CommandManager.RegisterClassInputBinding(typeof(PrestoSeekBar), new InputBinding(command, gestures[i]));
            }
        }

        static void OnIncreaseLargeCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is PrestoSeekBar prestoSeekBar)
                prestoSeekBar.OnIncreaseLarge();
        }

        static void OnDecreaseLargeCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is PrestoSeekBar prestoSeekBar)
                prestoSeekBar.OnDecreaseLarge();
        }

        static void OnIncreaseSmallCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is PrestoSeekBar prestoSeekBar)
                prestoSeekBar.OnIncreaseSmall();
        }

        static void OnDecreaseSmallCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is PrestoSeekBar prestoSeekBar)
                prestoSeekBar.OnDecreaseSmall();
        }

        static void OnMaximizeValueCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is PrestoSeekBar prestoSeekBar)
                prestoSeekBar.OnMaximizeValue();
        }

        static void OnMinimizeValueCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is PrestoSeekBar prestoSeekBar)
                prestoSeekBar.OnMinimizeValue();
        }

        void OnIncreaseLarge()
        {
            ValueOffset(LargeChange);
        }

        void OnDecreaseLarge()
        {
            ValueOffset(-LargeChange);
        }

        void OnIncreaseSmall()
        {
            ValueOffset(SmallChange);
        }

        void OnDecreaseSmall()
        {
            ValueOffset(-SmallChange);
        }

        void OnMaximizeValue()
        {
            SetCurrentValue(ValueProperty, Maximum);
        }

        void OnMinimizeValue()
        {
            SetCurrentValue(ValueProperty, Minimum);
        }
        #endregion

        #region Thumb Drag
        static void OnThumbDragStarted(object sender, DragStartedEventArgs e)
        {
            var seekBar = sender as PrestoSeekBar;
            seekBar.OnThumbDragStarted(e);
        }

        static void OnThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            var seekBar = sender as PrestoSeekBar;
            seekBar.OnThumbDragDelta(e);
        }

        static void OnThumbDragCompleted(object sender, DragCompletedEventArgs e)
        {
            var seekBar = sender as PrestoSeekBar;
            seekBar.OnThumbDragCompleted(e);
        }

        void OnThumbDragStarted(DragStartedEventArgs e)
        {
            if (IsTracking)
                BeginTracking();
        }

        void OnThumbDragDelta(DragDeltaEventArgs e)
        {
            var thumb = e.OriginalSource as Thumb;

            if (_track == null || _track.Thumb != thumb)
                return;

            double value = (IsTracking ? TrackValue : Value) + _track.ValueFromDistance(e.HorizontalChange, e.VerticalChange);

            if (double.IsInfinity(value) || double.IsNaN(value))
                return;

            value = Math.Min(Math.Max(Minimum, value), Maximum);

            if (IsTracking)
                SetTrackValue(value);
            else
                SetCurrentValue(ValueProperty, value);
        }

        void OnThumbDragCompleted(DragCompletedEventArgs e)
        {
            if (IsTracking)
                EndTracking();
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (IsTracking)
            {
                e.Handled = true;

                if (!_isDragging)
                {
                    if (_track?.Thumb != null)
                    {
                        Point position = e.GetPosition(_track.Thumb);
                        HitTestResult result = VisualTreeHelper.HitTest(_track.Thumb, position);

                        if (result != null)
                            e.Handled = false;
                    }

                    if (e.Handled)
                    {
                        _isDragging = true;
                        BeginTracking();
                        CaptureMouse();
                    }
                }
            }
            
            base.OnPreviewMouseLeftButtonDown(e);
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if (_isDragging)
            {
                if (e.MouseDevice.LeftButton == MouseButtonState.Pressed)
                {
                    Point position = e.GetPosition(_track);
                    double value = _track.ValueFromPoint(position);

                    SetTrackValue(value);
                }
                else
                {
                    if (e.MouseDevice.Captured == this)
                        ReleaseMouseCapture();

                    _isDragging = false;
                    EndTracking();
                }
            }

            base.OnPreviewMouseMove(e);
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (_isDragging)
            {
                if (IsMouseCaptured)
                    ReleaseMouseCapture();

                _isDragging = false;
                EndTracking();
            }

            base.OnPreviewMouseLeftButtonUp(e);
        }
        #endregion

        static void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
                return;

            if (sender is PrestoSeekBar seekBar && !seekBar.IsKeyboardFocusWithin)
                e.Handled = seekBar.Focus() || e.Handled;
        }

        static bool IsValidOrientation(object value)
        {
            var orientation = (Orientation)value;

            return orientation == Orientation.Horizontal || orientation == Orientation.Vertical;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _track = (WPFTrack)this.GetTemplateChild(TrackName);
        }

        void ValueOffset(double offset)
        {
            double value = Value + offset;
            value = Math.Min(Math.Max(Minimum, value), Maximum);

            SetCurrentValue(ValueProperty, value);
        }

        class SeekBarGesture : InputGesture
        {
            Key _normal;
            Key _inverted;
            bool _forHorizontal;

            public SeekBarGesture(Key normal, Key inverted, bool forHorizontal)
            {
                _normal = normal;
                _inverted = inverted;
                _forHorizontal = forHorizontal;
            }

            public override bool Matches(object targetElement, InputEventArgs inputEventArgs)
            {
                var keyEventArgs = inputEventArgs as KeyEventArgs;
                var seekBar = targetElement as PrestoSeekBar;

                if (keyEventArgs != null && seekBar != null && Keyboard.Modifiers == ModifierKeys.None)
                {
                    if (_normal == keyEventArgs.Key)
                        return !IsInverted(seekBar);

                    if (_inverted == keyEventArgs.Key)
                        return IsInverted(seekBar);
                }

                return false;
            }

            bool IsInverted(PrestoSeekBar seekBar)
            {
                if (_forHorizontal)
                {
                    return seekBar.IsDirectionReversed != (seekBar.FlowDirection == FlowDirection.RightToLeft);
                }
                else
                {
                    return seekBar.IsDirectionReversed;
                }
            }
        }
    }
}
