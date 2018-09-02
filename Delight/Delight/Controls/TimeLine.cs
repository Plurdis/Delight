using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Delight.Core.Extensions;
using Delight.Extensions;
using Delight.Timing;
using Delight.Timing.Common;

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

        public TimingReader TimingReader { get; }

        public TimeLine()
        {
            this.Style = FindResource("TimeLineStyle") as Style;
            
            Thread thr = new Thread(ThreadRun);
            thr.Start();

            ApplyTemplate();

            TimingReader = new TimingReader(this);
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

        public ReadOnlyCollection<Track> Tracks => 
            tracks.Children
                .Cast<Track>()
                .ToList()
                .AsReadOnly();

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
        public IEnumerable<TrackItem> Items => 
            tracks.Children
                .Cast<Track>()
                .SelectMany(i => i.Items);

        /// <summary>
        /// 해당 Track에 존재하는 아이템만 필터링합니다.
        /// </summary>
        /// <param name="items">필터링할 아이템들입니다.</param>
        /// <param name="track">필터링할 Track입니다.</param>
        /// <returns></returns>
        private IEnumerable<TrackItem> FilterItems(IEnumerable<TrackItem> items, Track track)
        {
            return items.Where(i => i.TemplatedParent == track);
        }

        /// <summary>
        /// 해당 TrackType에 해당하는 아이템만 필터링합니다.
        /// </summary>
        /// <param name="items">필터링할 아이템들입니다.</param>
        /// <param name="trackType">필터링할 TrackType입니다.</param>
        /// <returns></returns>
        private IEnumerable<TrackItem> FilterItems(IEnumerable<TrackItem> items, TrackType trackType)
        {
            return items.Where(i => i.TrackType == trackType);
        }

        /// <summary>
        /// 현재 TimeLine에 있는 아이템 중에 TrackType에 해당하는 아이템만 필터링합니다.
        /// </summary>
        /// <param name="trackType"></param>
        /// <returns></returns>
        public IEnumerable<TrackItem> GetItems(TrackType trackType)
        {
            return FilterItems(Items, trackType);
        }
        
        /// <summary>
        /// 현재 TimeLine에 있는 아이템 중에 frame 위치에 있는 아이템만 필터링합니다.
        /// </summary>
        /// <param name="frame">검색할 프레임입니다.</param>
        /// <param name="findType">검색할 조건입니다.</param>
        /// <returns></returns>
        public IEnumerable<TrackItem> GetItems(int frame, FindType findType = FindType.FindStartPoint)
        {
            switch (findType)
            {
                case FindType.FindStartPoint:
                    return Items.Where(i => i.Offset == frame);
                case FindType.FindEndPoint:
                    return Items.Where(i => i.Offset + i.FrameWidth == frame);
                case FindType.FindContains:
                    return Items.Where(i => i.Offset <= frame && i.Offset + i.FrameWidth >= frame);
                case FindType.FindWithoutStartPoint:
                    return Items.Where(i => i.Offset < frame && i.Offset + i.FrameWidth >= frame);
                case FindType.FindWithoutEndPoint:
                    return Items.Where(i => i.Offset <= frame && i.Offset + i.FrameWidth > frame);
                case FindType.FindWithoutStartEndPoint:
                    return Items.Where(i => i.Offset < frame && i.Offset + i.FrameWidth > frame);
                case FindType.FindAfterFrame:
                    return Items.Where(i => frame <= i.Offset);
                case FindType.FindAfterFrameExceptThis:
                    return Items.Where(i => frame < i.Offset);
                case FindType.FindBeforeFrame:
                    return Items.Where(i => frame >= i.Offset + i.FrameWidth);
                case FindType.FindBeforeFrameExceptThis:
                    return Items.Where(i => frame > i.Offset + i.FrameWidth);
                default:
                    return null;
            }
        }

        /// <summary>
        /// 현재 TimeLine에 있는 아이템 중에 frame 위치에 있고 TrackType이 같은 아이템만 필터링합니다.
        /// </summary>
        /// <param name="frame">검색할 프레임입니다.</param>
        /// <param name="trackType">필터링할 트랙의 형식입니다.</param>
        /// <param name="findType">검색할 조건입니다.</param>
        /// <returns></returns>
        public IEnumerable<TrackItem> GetItems(int frame, TrackType trackType, FindType findType = FindType.FindStartPoint)
        {
            var itm = FilterItems(Items, trackType);
            switch (findType)
            {
                case FindType.FindStartPoint:
                    return itm.Where(i => i.Offset == frame);
                case FindType.FindEndPoint:
                    return itm.Where(i => i.Offset + i.FrameWidth == frame);
                case FindType.FindContains:
                    return itm.Where(i => i.Offset <= frame && i.Offset + i.FrameWidth >= frame);
                case FindType.FindWithoutStartPoint:
                    return itm.Where(i => i.Offset < frame && i.Offset + i.FrameWidth >= frame);
                case FindType.FindWithoutEndPoint:
                    return itm.Where(i => i.Offset <= frame && i.Offset + i.FrameWidth > frame);
                case FindType.FindWithoutStartEndPoint:
                    return itm.Where(i => i.Offset < frame && i.Offset + i.FrameWidth > frame);
                case FindType.FindAfterFrame:
                    return itm.Where(i => frame <= i.Offset);
                case FindType.FindAfterFrameExceptThis:
                    return itm.Where(i => frame < i.Offset);
                case FindType.FindBeforeFrame:
                    return itm.Where(i => frame >= i.Offset + i.FrameWidth);
                case FindType.FindBeforeFrameExceptThis:
                    return itm.Where(i => frame > i.Offset + i.FrameWidth);
                default:
                    return null;
            }
        }

        /// <summary>
        /// 현재 TimeLine에 있는 아이템 중에 startFrame과 endFrame 사이에 있는 아이템들을 FindRangeType 조건에 따라 필터링합니다.
        /// </summary>
        /// <param name="startFrame">검색할 프레임의 시작점입니다.</param>
        /// <param name="endFrame">검색할 프레임의 종료점입니다.</param>
        /// <param name="findRangeType">검색할 조건입니다.</param>
        /// <returns></returns>
        public IEnumerable<TrackItem> GetItems(int startFrame, int endFrame, FindRangeType findRangeType = FindRangeType.FindContains)
        {
            switch (findRangeType)
            {
                case FindRangeType.FindExcatly:
                    return Items.Where(i => startFrame == i.Offset && endFrame == i.Offset + i.FrameWidth);
                case FindRangeType.FindStartPoint:
                    return Items.Where(i => startFrame <= i.Offset && endFrame >= i.Offset);
                case FindRangeType.FindEndPoint:
                    return Items.Where(i => startFrame <= i.Offset + i.FrameWidth && endFrame >= i.Offset + i.FrameWidth);
                case FindRangeType.FindContains:
                    return Items.Where(i => startFrame <= i.Offset && endFrame >= i.Offset + i.FrameWidth);
                default:
                    return null;
            }
        }        
        /// <summary>
        /// 현재 TimeLine에 있는 아이템 중에 startFrame과 endFrame 사이에 있는 아이템들을 FindRangeType 조건에 따라 필터링합니다.
        /// </summary>
        /// <param name="startFrame">검색할 프레임의 시작점입니다.</param>
        /// <param name="endFrame">검색할 프레임의 종료점입니다.</param>
        /// <param name="findRangeType">검색할 조건입니다.</param>
        /// <returns></returns>
        public IEnumerable<TrackItem> GetItems(int startFrame, int endFrame, TrackType trackType, FindRangeType findRangeType = FindRangeType.FindContains)
        {
            var itm = FilterItems(Items, trackType);
            switch (findRangeType)
            {
                case FindRangeType.FindExcatly:
                    return itm.Where(i => startFrame == i.Offset && endFrame == i.Offset + i.FrameWidth);
                case FindRangeType.FindStartPoint:
                    return itm.Where(i => startFrame <= i.Offset && endFrame >= i.Offset);
                case FindRangeType.FindEndPoint:
                    return itm.Where(i => startFrame <= i.Offset + i.FrameWidth && endFrame >= i.Offset + i.FrameWidth);
                case FindRangeType.FindContains:
                    return itm.Where(i => startFrame <= i.Offset && endFrame >= i.Offset + i.FrameWidth);
                default:
                    return null;
            }
        }

        //public IEnumerable<TrackItem> GetItems(TrackType trackType)
        //{
        //    return Items.Where(i => i.TrackType == trackType);
        //}

        //public IEnumerable<TrackItem> GetItems(TrackType trackType, int startFrame)
        //{
        //    return Items.Where(i => (i.TrackType == trackType) && (i.Offset >= startFrame || i.Offset + i.FrameWidth > startFrame));
        //}

        ///// <summary>
        ///// 아이템들이 지정된 TrackType이며 시작 프레임과 종료 프레임에 걸처있는지에 대한 여부를 확인합니다.
        ///// </summary>
        ///// <param name="trackType"></param>
        ///// <param name="startFrame"></param>
        ///// <param name="endFrame"></param>
        ///// <returns></returns>
        //public IEnumerable<TrackItem> GetItems(TrackType trackType, int startFrame, int endFrame)
        //{
        //    return Items.Where(i => (i.TrackType == trackType)
        //    && (i.Offset >= startFrame || i.Offset + i.FrameWidth > startFrame)
        //    );
        //}

        ///// <summary>
        ///// 모든 Track에 있는 모든 아이템들을 가져옵니다.
        ///// </summary>
        ///// <returns></returns>
        //public IEnumerable<TrackItem> GetItems()
        //{
        //    return Items;
        //}

        ///// <summary>
        ///// 아이템들이 frame에 걸쳐 있는지에 대한 여부를 확인합니다.
        ///// </summary>
        ///// <param name="frame">확인할 프레임입니다.</param>
        ///// <returns></returns>
        //public IEnumerable<TrackItem> GetItemsInclude(int frame)
        //{
        //    return Items.Where(i => i.Offset <= frame && i.FrameWidth + i.Offset > frame);
        //}

        //public IEnumerable<TrackItem> GetItemsAtStart(int frame, TrackType trackType)
        //{
        //    return Items.Where(i => i.TrackType == trackType && i.Offset == frame);
        //}

        //public IEnumerable<TrackItem> GetItemsAtStart(int frame)
        //{
        //    return Items.Where(i => i.Offset == frame);
        //}

        //public IEnumerable<TrackItem> GetItemsAtEnd(int frame)
        //{
        //    return Items.Where(i => i.Offset + i.FrameWidth == frame);
        //}

        //public IEnumerable<TrackItem> GetItemsAtEnd(int frame, TrackType trackType)
        //{
        //    return Items.Where(i => trackType == i.TrackType && i.Offset + i.FrameWidth == frame);
        //}

        #endregion

        #region [  Track Management (Add/Remove)  ]

        public void AddTrack(TrackType trackType, int number = -1)
        {
            Track track = new Track(this, trackType);

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

        public void RemoveTrack(int index)
        {

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
                TimingReader.StartLoad();
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
            TimingReader.StopLoad();
            TimeLineStoped?.Invoke(this, new EventArgs());
            _timer.Stop();
        }

        public bool isPaused = false;

        public void Pause()
        {
            _timer.Stop();
            isPaused = true;
        }

        public void Resume()
        {
            _timer.Start();
            isPaused = false;
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
