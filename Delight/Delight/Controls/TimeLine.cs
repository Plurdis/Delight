using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

using Delight.Common;
using Delight.Components.Common;
using Delight.Core.Common;
using Delight.Extensions;
using Delight.Timing;

namespace Delight.Controls
{
    [TemplatePart(Name = "positioner", Type = typeof(Rectangle))]
    [TemplatePart(Name = "dragRange", Type = typeof(Grid))]
    [TemplatePart(Name = "scrollBar", Type = typeof(ScrollBar))]
    [TemplatePart(Name = "tracks", Type = typeof(StackPanel))]
    public class TimeLine : Control
    {
        public event EventHandler FrameChanged;
        public event EventHandler FrameMouseChanged;
        
        public event EventHandler<ItemEventArgs> ItemAdded;
        public event EventHandler<ItemEventArgs> ItemRemoving;
        public event EventHandler ItemRemoved;

        public event EventHandler TimeLineStarted;
        public event EventHandler TimeLineStoped;

        Rectangle positioner;
        Grid dragRange;
        ScrollBar scrollBar;
        StackPanel tracks;

        // Drag Movement에서 쓰일 변수들
        bool captured = false;
        UIElement source;
        double absLeft, relLeft;
        FrameTimer _timer;

        public TimingReader TimeLineReader { get; }

        public TimeLine()
        {
            this.Style = FindResource("TimeLineStyle") as Style;
            
            Thread thr = new Thread(ThreadRun);
            thr.Start();

            TimeLineReader = new TimingReader(this);
        }

        public void ThreadRun()
        {
            _timer = new FrameTimer(FrameRate);
            _timer.Tick += () =>
            {
                Dispatcher.Invoke(() =>
                {
                    Position++;
                });
            };
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            positioner = GetTemplateChild("positioner") as Rectangle;
            dragRange = GetTemplateChild("dragRange") as Grid;
            scrollBar = GetTemplateChild("scrollBar") as ScrollBar;
            tracks = GetTemplateChild("tracks") as StackPanel;

            this.MouseWheel += TimeLine_MouseWheel;
            scrollBar.Scroll += ScrollBar_Scroll;

            dragRange.MouseLeftButtonDown += DragRange_MouseLeftButtonDown;
            dragRange.MouseLeftButtonUp += DragRange_MouseLeftButtonUp;
            dragRange.MouseMove += DragRange_MouseMove;

            var prop = DependencyPropertyDescriptor.FromProperty(ScrollBar.ValueProperty, typeof(ScrollBar));

            this.SizeChanged += (s, e) =>
            {
                tracks.Children.Cast<Track>().ForEach(i => i.RelocationTrackItems());
                Track_ItemsMaxWidthChanged(null, null);
            };

            prop.AddValueChanged(scrollBar, ScrollBarValueChanged);

            AddTrack(TrackType.Image);
            AddTrack(TrackType.Video);
            AddTrack(TrackType.Effect,1);
            AddTrack(TrackType.Effect,2);
            AddTrack(TrackType.Effect,3);
            AddTrack(TrackType.Sound);
            AddTrack(TrackType.Light, 1);
            AddTrack(TrackType.Light, 2);
            AddTrack(TrackType.Light, 3);
        }

        #region [  Properties  ]

        public static DependencyProperty PositionProperty = DependencyProperty.Register(nameof(Position), typeof(int), typeof(TimeLine), new PropertyMetadata(0, PositionChanged));

        private static void PositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TimeLine timeLine)
            {
                timeLine.FrameChanged?.Invoke(timeLine, new EventArgs());
            }
        }

        public int Position
        {
            get => (int)GetValue(PositionProperty);
            set
            {
                if (value > MaxFrame)
                {
                    value = MaxFrame;
                    Stop();
                }
                    
                SetValue(PositionProperty, value);
                SetPositionerToPosition();
            }
        }

        private void SetPositionerToPosition()
        {
            positioner.Margin = new Thickness(Position * _realSize - 0.5 - Offset, 0, 0, 0);
        }

        public const double ItemSize = 0.25;

        public double Offset => scrollBar.Value;

        private int Weight => GetWeight(Ratio);

        private double _realSize => ItemSize * Ratio;

        private double _displaySize => _realSize * Weight;


        private const double MaxRatio = 25.6;


        public static DependencyProperty RatioProperty = DependencyProperty.Register(nameof(Ratio),
            typeof(double),
            typeof(TimeLine),
            new FrameworkPropertyMetadata(0.4, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public double Ratio
        {
            get => (double)GetValue(RatioProperty);
            private set
            {
                if (value < 0.1)
                    value = 0.1;
                if (value > MaxRatio)
                    value = MaxRatio;

                SetValue(RatioProperty, value);
                SetPositionerToPosition();


                scrollBar.ViewportSize = value * 100;
                Track_ItemsMaxWidthChanged(null, null);

                tracks.Children.Cast<Track>().ForEach(i => i.RelocationTrackItems());

                //ResetItemOffset();
            }
        }

        public double MaxValue { get; set; }

        public int MaxFrame { get; set; }

        public bool IsRunning => _timer.IsRunning;

        FrameRate _frameRate;

        public FrameRate FrameRate
        {
            get => _frameRate;
            set
            {
                _frameRate = value;
                _timer.FrameRate = value;
            }
        }

        #endregion

        private void ScrollBarValueChanged(object sender, EventArgs e)
        {
            tracks.Children.Cast<Track>().ForEach(i => i.RelocationTrackItems());
            this.InvalidateVisual();
        }

        private void ScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
        }

        private void TimeLine_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                double inc = GetIncrement(Ratio);
                if (Keyboard.IsKeyDown(Key.LeftShift))
                    inc *= 2;

                if (e.Delta > 0)
                    Ratio += inc;
                else
                    Ratio -= inc;
            }
            else
            {
                if (scrollBar.Visibility == Visibility.Visible)
                {
                    scrollBar.Value -= (e.Delta * Ratio);
                    ScrollBar_Scroll(scrollBar, null);
                }
            }
        }

        #region [  TrackItems Management  ]

        public IEnumerable<TrackItem> Items => tracks.Children.Cast<Track>().SelectMany(i => i.Items);

        public IEnumerable<TrackItem> GetItems(TrackType trackType)
        {
            return Items.Where(i => i.TrackType == trackType);
        }

        public IEnumerable<TrackItem> GetItems(TrackType trackType, int startFrame)
        {
            return Items.Where(i => (i.TrackType == trackType) && (i.Offset >= startFrame || i.Offset + i.FrameWidth > startFrame));
        }

        public IEnumerable<TrackItem> GetItems(TrackType trackType, int startFrame, int endFrame)
        {
            return Items.Where(i => (i.TrackType == trackType)
            && (i.Offset >= startFrame || i.Offset + i.FrameWidth > startFrame)
            );
        }

        public IEnumerable<TrackItem> GetItemsInclude(int frame)
        {
            return Items.Where(i => i.Offset <= frame && i.FrameWidth + i.Offset > frame);
        }

        public IEnumerable<TrackItem> GetItemsAtStart(int frame, TrackType trackType)
        {
            return Items.Where(i => i.TrackType == trackType && i.Offset == frame);
        }

        public IEnumerable<TrackItem> GetItemsAtStart(int frame)
        {
            return Items.Where(i => i.Offset == frame);
        }

        public IEnumerable<TrackItem> GetItemsAtEnd(int frame)
        {
            return Items.Where(i => i.Offset + i.FrameWidth == frame);
        }

        public IEnumerable<TrackItem> GetItemsAtEnd(int frame, TrackType trackType)
        {
            return Items.Where(i => trackType == i.TrackType && i.Offset + i.FrameWidth == frame);
        }



        #endregion

        #region [  Track Management (Add/Remove)  ]

        public void AddTrack(TrackType trackType, int number = -1)
        {
            Track track = new Track(this, trackType, FrameRate);

            if (number != -1)
            {
                track.TrackNumber = number;
            }

            track.ItemsMaxWidthChanged += Track_ItemsMaxWidthChanged;

            track.ItemAdded += Track_ItemAdded;
            track.ItemRemoving += Track_ItemRemoving;
            track.ItemRemoved += Track_ItemRemoved;

            tracks.Children.Add(track);
        }

        private void Track_ItemRemoved(object sender, EventArgs e)
        {
            ItemRemoved?.Invoke(sender, e);
        }

        private void Track_ItemRemoving(object sender, ItemEventArgs e)
        {
            ItemRemoving?.Invoke(sender, e);
        }

        private void Track_ItemAdded(object sender, ItemEventArgs e)
        {
            ItemAdded?.Invoke(sender, e);
        }

        private void Track_ItemsMaxWidthChanged(object sender, EventArgs e)
        {
            MaxFrame = tracks.Children.Cast<Track>().Max(i => i.ItemsMaxFrame);

            if (this.Position > MaxFrame)
                this.Position = MaxFrame;

            MaxValue = tracks.Children.Cast<Track>().Max(i => i.ItemsMaxWidth);

            double value = MaxValue - (this.ActualWidth - 184);
            if (value > 0)
            {
                scrollBar.Maximum = value + 1;
                scrollBar.Visibility = Visibility.Visible;
            }
            else
            {
                scrollBar.Value = 0;
                scrollBar.Maximum = 0;
                scrollBar.Visibility = Visibility.Hidden;
            }
            
            
        }

        #endregion

        #region [  Positioner Movement  ]

        public bool IsReady { get; set; } = false;

        public void Play()
        {
            //await Task.Factory.StartNew(() => {  });
            Task.Run(() =>
            {
                IsReady = true;
                TimeLineReader.StartLoad();
                DebugHelper.WriteLine("┌────────┐");
                DebugHelper.WriteLine("  Timer Started!");
                DebugHelper.WriteLine("└────────┘");
                TimeLineStarted?.Invoke(this, new EventArgs());
                _timer.Start();
                IsReady = false;
            }); 
        }

        public void Stop()
        {
            //  test
            TimeLineReader.StopLoad();
            TimeLineStoped?.Invoke(this, new EventArgs());
            _timer.Stop();
        }

        private void DragRange_MouseMove(object sender, MouseEventArgs e)
        {
            if (captured)
            {
                double x = e.GetPosition(this).X;
                relLeft += x - absLeft;

                SetSliderLeft();

                absLeft = x;
            }
        }

        private void DragRange_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            captured = false;
        }

        private void DragRange_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            source = (UIElement)sender;
            Mouse.Capture(source);
            captured = true;
            absLeft = e.GetPosition(this).X;
            relLeft = GetSliderLeft() + Mouse.GetPosition(positioner).X;

            SetSliderLeft();
        }

        private void SetSliderLeft()
        {
            double left = relLeft + Offset;
            if (left < 0)
                left = 0;

            var frame = (int)(left / _realSize);
            

            if (frame == 0 || frame != Position)
            {
                SetPositionerToPosition();
            }
            if (frame != Position)
            {
                Position = frame;
                FrameMouseChanged?.Invoke(this, new EventArgs());
            }
        }
        
        public double GetSliderLeft()
        {
            return positioner.Margin.Left;
        }

        #endregion

        protected override void OnRender(DrawingContext dc)
        {
            dc.DrawRectangle(Brushes.Transparent, null, new Rect(0, 0, this.ActualWidth, this.ActualHeight));
            DrawHelperLine(dc);
            base.OnRender(dc);
        }

        #region [  Ratio  ]

        public int GetWeight(double ratio)
        {
            return (int)GetWeightOrIncrement(ratio, true);
        }

        public double GetIncrement(double ratio)
        {
            return GetWeightOrIncrement(ratio, false);
        }

        private double GetWeightOrIncrement(double ratio, bool getWeight)
        {
            double f = 0.1, s = f * 2;

            int weight = (int)Math.Pow(2, 8);

            while (f != MaxRatio)
            {
                if ((f <= ratio) && (ratio < s))
                {
                    return getWeight ? weight : f / 10;
                }
                weight /= 2;
                f = s;
                s *= 2;
            }

            if (ratio == f)
                return getWeight ? weight : f / 10;

            return -1;
        }

        public void DrawHelperLine(DrawingContext dc)
        {
            int startPoint = 184;

            positioner.SetLeftMargin(Position * _realSize - Offset);

            double sizeOffset = Offset == 0 ? 0 : (_displaySize - (Offset % _displaySize)) - _displaySize;
            int value = (int)(Offset / _displaySize);

            for (int i = 0; i <= (ActualWidth - startPoint) / _displaySize; i++)
            {
                var pen = new Pen(Brushes.Gray, 1);

                double halfPenWidth = pen.Thickness / 2;

                GuidelineSet guidelines = new GuidelineSet();

                guidelines.GuidelinesX.Add(startPoint + (i * _displaySize) + sizeOffset + halfPenWidth);
                guidelines.GuidelinesX.Add(startPoint + (i * _displaySize) + sizeOffset + _displaySize + halfPenWidth);

                dc.PushGuidelineSet(guidelines);
                int height = 10;
                if ((i + value) % 12 == 0)
                {
                    height = 28;
                    pen.Brush = Brushes.White;
                    dc.DrawText(new FormattedText(MediaTools.GetTimeText((i + value) * Weight, FrameRate), CultureInfo.GetCultureInfo("en-us"),
                        FlowDirection.LeftToRight,
                        new Typeface("/Delight;component/Resources/Fonts/#Helvetica"),
                        10, Brushes.White),
                        new Point(startPoint + i * _displaySize + sizeOffset + 4, 20));

                    guidelines.GuidelinesY.Add(35.5 - height);
                    //DrawLine(dc, pen, new Point(startPoint + i * _displaySize + sizeOffset, 35 - height), new Point(startPoint + i * _displaySize + sizeOffset + 3, 35 - height));
                }
                else if ((i + value) % 4 == 0)
                {
                    height = 20;
                    pen.Brush = Brushes.White;
                }
                DrawLine(dc, pen, new Point(startPoint + i * _displaySize + sizeOffset, 0), new Point(startPoint + i * _displaySize + sizeOffset, height));
                dc.Pop();
            }
        }

        public void DrawLine(DrawingContext dc, Pen p, Point p1, Point p2)
        {
            if (p1.X >= 184)
            {
                dc.DrawLine(p, p1, p2);
            }
        }

        #endregion
    }
}
