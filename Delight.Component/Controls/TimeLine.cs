using Delight.Component.Common;
using Delight.Component.Extensions;
using Delight.Component.Primitives;
using Delight.Component.Primitives.Controllers;
using Delight.Component.Primitives.TimingReaders;
using Delight.Core.Attributes;
using Delight.Core.Common;
using Delight.Core.Extensions;
using Delight.Core.Stage;
using Delight.Core.Stage.Components;
using Delight.Core.Timers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Delight.Component.Controls
{
    // TODO: 코드 정리

    [TemplatePart(Name = "positioner", Type = typeof(Rectangle))]
    [TemplatePart(Name = "dragRange", Type = typeof(Grid))]
    [TemplatePart(Name = "scrollBar", Type = typeof(ScrollBar))]
    [TemplatePart(Name = "tracks", Type = typeof(StackPanel))]
    public class TimeLine : Control
    {
        public event EventHandler FrameChanged;
        public event EventHandler FrameMouseChanged;

        public void OnFrameMouseChanged(object sender, EventArgs e)
        {
            FrameMouseChanged?.Invoke(sender, e);
        }

        public event EventHandler<ItemEventArgs> ItemAdded;
        public event EventHandler<ItemEventArgs> ItemRemoving;
        public event EventHandler ItemRemoved;

        public event EventHandler TimeLineStarted;
        public event EventHandler TimeLineStopped;

        public event EventHandler<TrackEventArgs> TrackAdded;
        public event EventHandler<TrackEventArgs> TrackRemoved;

        public event EventHandler ItemSelected;
        public event EventHandler ItemDeselected;

        Rectangle positioner;
        Grid dragRange;
        ScrollBar scrollBar;
        StackPanel visualTracks, otherTracks;

        // Drag Movement에서 쓰일 변수들
        bool captured = false;
        UIElement source;
        double absLeft, relLeft;
        FrameTimer _timer;

        public TrackItem SelectedItem { get; private set; }

        public Dictionary<Track, TimingReader> TimingReaders { get; }
        
        public Dictionary<Track, BaseController> Controllers { get; }
        public TimeLine()
        {
            this.Style = FindResource("TimeLineStyle") as Style;

            Thread thr = new Thread(ThreadRun);
            thr.Start();

            TimingReaders = new Dictionary<Track, TimingReader>();
            Controllers = new Dictionary<Track, BaseController>();

            ApplyTemplate();

            scrollBar.Style = FindResource("TimeLineScrollBar") as Style;

            FrameMouseChanged += delegate
            {
                Stop();
            };
        }

        public void ThreadRun()
        {
            _timer = new FrameTimer(FrameRate);
            _timer.Tick += (s,e) =>
            {
                Dispatcher.Invoke(() =>
                {
                    Position++;
                });
            };
        }

        public void Reset()
        {
            visualTracks.Children.Clear();
            otherTracks.Children.Clear();
        }

        public List<ItemPosition> ExportData()
        {
            var items = new List<ItemPosition>();

            foreach (TrackItem item in AllItems)
            {
                var itemPosition = new ItemPosition()
                {
                    BackwardOffset = item.BackwardOffset,
                    ForwardOffset = item.ForwardOffset,
                    Offset = item.Offset,
                    ItemId = item.GetTag<StageComponent>().Id,
                    SourceType = item.SourceType,
                    TrackNumber = ((((FrameworkElement)item.Parent).TemplatedParent) as Track).TrackNumber
                };

                items.Add(itemPosition);
            }

            return items;
        }

        public void ImportData(List<ItemPosition> items, IEnumerable<StageComponent> MediaItems)
        {
            Dictionary<string, StageComponent> dictionary = MediaItems.ToDictionary(i => i.Id, i => i);

            foreach(ItemPosition item in items)
            {
                StageComponent component = dictionary[item.ItemId];
                AddItem(component, item);
            }

            Tracks.ForEach(i => i.RelocationTrackItems());
        }

        private void AddItem(StageComponent component, ItemPosition itemPosition)
        {
            var track = Tracks.Where(i => i.SourceType == itemPosition.SourceType && i.TrackNumber == itemPosition.TrackNumber).FirstOrDefault();

            if (track == null)
            {
                track = AddTrack(itemPosition.SourceType, itemPosition.TrackNumber);
            }

            TrackItem item = track.BuildItem(component);

            item.Offset = itemPosition.Offset;
            item.ForwardOffset = itemPosition.ForwardOffset;
            item.BackwardOffset = itemPosition.BackwardOffset;

            track.AddItem(item);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            positioner = GetTemplateChild("positioner") as Rectangle;
            dragRange = GetTemplateChild("dragRange") as Grid;
            scrollBar = GetTemplateChild("scrollBar") as ScrollBar;
            visualTracks = GetTemplateChild("visualTracks") as StackPanel;
            otherTracks = GetTemplateChild("otherTracks") as StackPanel;


            this.MouseWheel += TimeLine_MouseWheel;
            scrollBar.Scroll += ScrollBar_Scroll;

            dragRange.MouseLeftButtonDown += DragRange_MouseLeftButtonDown;
            dragRange.MouseLeftButtonUp += DragRange_MouseLeftButtonUp;
            dragRange.MouseMove += DragRange_MouseMove;

            this.PreviewMouseDown += TimeLine_PreviewMouseDown;

            this.SizeChanged += (s, e) =>
            {
                Tracks.ForEach(i => i.RelocationTrackItems());
                Track_ItemsMaxWidthChanged(null, null);
            };
        }

        private void TimeLine_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = null;

            try
            {
                element = ((FrameworkElement)e.OriginalSource).TemplatedParent as FrameworkElement;
            }
            catch (InvalidCastException)
            {
                return;
            }

            AllItems.ForEach(i => i.IsSelected = false);
            SelectedItem = null;
            if (element is TrackItem trackItem)
            {
                trackItem.IsSelected = true;
                SelectedItem = trackItem;
                ItemSelected?.Invoke(trackItem, new EventArgs());
            }
            else
            {
                ItemDeselected?.Invoke(null, new EventArgs());
            }
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

                Tracks.ForEach(i => i.RelocationTrackItems());

                //ResetItemOffset();
            }
        }

        public static DependencyProperty HeaderFontFamilyProperty = DependencyProperty.Register(nameof(HeaderFontFamily), typeof(FontFamily), typeof(TimeLine));

        public FontFamily HeaderFontFamily
        {
            get => GetValue(HeaderFontFamilyProperty) as FontFamily;
            set => SetValue(HeaderFontFamilyProperty, value);
        }

        public double MaxValue { get; set; }

        public int MaxFrame { get; set; }

        public bool IsRunning => _timer.IsRunning;

        public ReadOnlyCollection<Track> VisualTracks =>
            visualTracks.Children
                .Cast<Track>()
                .ToList()
                .AsReadOnly();

        public ReadOnlyCollection<Track> NotVisualTracks =>
            otherTracks.Children
                .Cast<Track>()
                .ToList()
                .AsReadOnly();

        public IEnumerable<Track> Tracks => VisualTracks.Concat(NotVisualTracks);

        public int GetVisualTrackIndex(Track track)
        {
            return visualTracks.Children.IndexOf(track);
        }

        public int GetNotvisualTrackIndex(Track track)
        {
            return otherTracks.Children.IndexOf(track);
        }

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

        private void ScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            Tracks.ForEach(i => i.RelocationTrackItems());
            this.InvalidateVisual();
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

        public IEnumerable<TrackItem> AllItems =>
            Tracks.SelectMany(i => i.Items);


        #region [  TrackItems Management  ]

        /// <summary>
        /// 해당 Track에 존재하는 아이템만 필터링합니다.
        /// </summary>
        /// <param name="items">필터링할 아이템들입니다.</param>
        /// <param name="track">필터링할 Track입니다.</param>
        /// <returns></returns>
        private IEnumerable<TrackItem> FilterItems(IEnumerable<TrackItem> items, Track track)
        {
            return items.Where(i =>
            {
                return ((Grid)i.Parent).TemplatedParent == track;
            });
        }

        /// <summary>
        /// 해당 SourceType에 해당하는 아이템만 필터링합니다.
        /// </summary>
        /// <param name="items">필터링할 아이템들입니다.</param>
        /// <param name="SourceType">필터링할 SourceType입니다.</param>
        /// <returns></returns>
        private IEnumerable<TrackItem> FilterItems(IEnumerable<TrackItem> items, SourceType SourceType)
        {
            return items.Where(i => i.SourceType == SourceType);
        }

        /// <summary>
        /// 현재 TimeLine에 있는 아이템 중에 SourceType에 해당하는 아이템만 필터링합니다.
        /// </summary>
        /// <param name="SourceType"></param>
        /// <returns></returns>
        public IEnumerable<TrackItem> GetItems(SourceType SourceType)
        {
            return FilterItems(AllItems, SourceType);
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
                    return AllItems.Where(i => i.Offset == frame);
                case FindType.FindEndPoint:
                    return AllItems.Where(i => i.Offset + i.FrameWidth == frame);
                case FindType.FindContains:
                    return AllItems.Where(i => i.Offset <= frame && i.Offset + i.FrameWidth >= frame);
                case FindType.FindWithoutStartPoint:
                    return AllItems.Where(i => i.Offset < frame && i.Offset + i.FrameWidth >= frame);
                case FindType.FindWithoutEndPoint:
                    return AllItems.Where(i => i.Offset <= frame && i.Offset + i.FrameWidth > frame);
                case FindType.FindWithoutStartEndPoint:
                    return AllItems.Where(i => i.Offset < frame && i.Offset + i.FrameWidth > frame);
                case FindType.FindAfterFrame:
                    return AllItems.Where(i => frame <= i.Offset);
                case FindType.FindAfterFrameExceptThis:
                    return AllItems.Where(i => frame < i.Offset);
                case FindType.FindBeforeFrame:
                    return AllItems.Where(i => frame >= i.Offset + i.FrameWidth);
                case FindType.FindBeforeFrameExceptThis:
                    return AllItems.Where(i => frame > i.Offset + i.FrameWidth);
                default:
                    return null;
            }
        }

        /// <summary>
        /// 현재 TimeLine에 있는 아이템 중에 frame 위치에 있고 SourceType이 같은 아이템만 필터링합니다.
        /// </summary>
        /// <param name="frame">검색할 프레임입니다.</param>
        /// <param name="SourceType">필터링할 트랙의 형식입니다.</param>
        /// <param name="findType">검색할 조건입니다.</param>
        /// <returns></returns>
        public IEnumerable<TrackItem> GetItems(int frame, SourceType SourceType, FindType findType = FindType.FindStartPoint)
        {
            var itm = FilterItems(AllItems, SourceType);
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
        /// 현재 TimeLine에 있는 아이템 중에 frame 위치에 있고 같은 Track이며 SourceType이 같은 아이템만 필터링합니다.
        /// </summary>
        /// <param name="frame">검색할 프레임입니다.</param>
        /// <param name="SourceType">필터링할 트랙의 형식입니다.</param>
        /// <param name="findType">검색할 조건입니다.</param>
        /// <returns></returns>
        public IEnumerable<TrackItem> GetItems(int frame, Track track, FindType findType = FindType.FindStartPoint)
        {
            var itm = FilterItems(AllItems, track);
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
                    return AllItems.Where(i => startFrame == i.Offset && endFrame == i.Offset + i.FrameWidth);
                case FindRangeType.FindStartPoint:
                    return AllItems.Where(i => startFrame <= i.Offset && endFrame >= i.Offset);
                case FindRangeType.FindEndPoint:
                    return AllItems.Where(i => startFrame <= i.Offset + i.FrameWidth && endFrame >= i.Offset + i.FrameWidth);
                case FindRangeType.FindContains:
                    return AllItems.Where(i => startFrame <= i.Offset && endFrame >= i.Offset + i.FrameWidth);
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
        public IEnumerable<TrackItem> GetItems(int startFrame, int endFrame, SourceType SourceType, FindRangeType findRangeType = FindRangeType.FindContains)
        {
            var itm = FilterItems(AllItems, SourceType);
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

        /// <summary>
        /// 현재 TimeLine에 있는 아이템 중에 startFrame과 endFrame 사이에 있는 아이템들을 FindRangeType 조건에 따라 필터링합니다.
        /// </summary>
        /// <param name="startFrame">검색할 프레임의 시작점입니다.</param>
        /// <param name="endFrame">검색할 프레임의 종료점입니다.</param>
        /// <param name="findRangeType">검색할 조건입니다.</param>
        /// <returns></returns>
        public IEnumerable<TrackItem> GetItems(int startFrame, int endFrame, Track track, FindRangeType findRangeType = FindRangeType.FindContains)
        {
            var itm = FilterItems(AllItems, track);
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

        #endregion

        #region [  Track Management (Add/Remove)  ]

        /// <summary>
        /// 트랙을 추가합니다.
        /// </summary>
        /// <param name="SourceType">추가할 트랙의 형식입니다.</param>
        /// <param name="number">숫자입니다.</param>
        public Track AddTrack(SourceType SourceType, int number = -1)
        {
            Track track = new Track(this, SourceType);

            if (number != -1)
            {
                track.TrackNumber = number;
            }

            track.ItemsMaxWidthChanged += Track_ItemsMaxWidthChanged;

            track.ItemAdded += Track_ItemAdded;
            track.ItemRemoving += Track_ItemRemoving;
            track.ItemRemoved += Track_ItemRemoved;

            if (SourceType.GetAttribute<OutputDeviceAttribute>()?.OutputDevice == OutputDevice.Display)
            {
                visualTracks.Children.Add(track);

                if (SourceType == SourceType.Video)
                {
                    TimingReaders[track] = new DelayTimingReader(this, track);
                    Controllers[track] = new VideoController(track, (DelayTimingReader)TimingReaders[track]);
                }
                else if (SourceType == SourceType.Image)
                {
                    var reader = new TimingReader(this, track);
                    TimingReaders[track] = reader;
                    Controllers[track] = new ImageController(track, reader);
                }
            }
            else
            {
                if (SourceType == SourceType.Sound)
                {
                    var reader = new TimingReader(this, track);
                    TimingReaders[track] = reader;
                    Controllers[track] = new SoundController(track, reader);
                }
                else if (SourceType == SourceType.Light)
                {
                    var reader = new TimingReader(this, track);
                    TimingReaders[track] = reader;
                    Controllers[track] = new LightController(track, reader);
                }
                otherTracks.Children.Add(track);
            }

            TrackAdded?.Invoke(this, new TrackEventArgs(track, SourceType));

            return track;
        }

        public void RemoveTrack(int index, bool isVisualTrack)
        {
            StackPanel grid = isVisualTrack ? visualTracks : otherTracks;
            Track track = grid.Children[index] as Track;
            TrackRemoved?.Invoke(this, new TrackEventArgs(track, track.SourceType));

            grid.Children.RemoveAt(index);
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
            if (Tracks.Count() == 0)
                return;

            MaxFrame = Tracks.Max(i => i.ItemsMaxFrame);

            if (this.Position > MaxFrame)
                this.Position = MaxFrame;

            MaxValue = Tracks.Max(i => i.ItemsMaxWidth);

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

                TimingReaders
                    .ForEach(i =>
                    {
                        if (i.Value is DelayTimingReader dReader)
                        {
                            dReader.StartLoad();
                        }
                    });

                Console.WriteLine("┌────────┐");
                Console.WriteLine("  Timer Started!");
                Console.WriteLine("└────────┘");
                TimeLineStarted?.Invoke(this, new EventArgs());
                _timer.Start();
                IsReady = false;
            });
        }

        public void Stop()
        {
            //  test
            TimingReaders
                .ForEach(i =>
                {
                    if (i.Value is DelayTimingReader dReader)
                    {
                        dReader.StopLoad();
                    }
                });

            TimeLineStopped?.Invoke(this, new EventArgs());
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
                var pen = new Pen(Brushes.Black, 1);

                double halfPenWidth = pen.Thickness / 2;

                GuidelineSet guidelines = new GuidelineSet();

                guidelines.GuidelinesX.Add(startPoint + (i * _displaySize) + sizeOffset + halfPenWidth);
                guidelines.GuidelinesX.Add(startPoint + (i * _displaySize) + sizeOffset + _displaySize + halfPenWidth);

                dc.PushGuidelineSet(guidelines);
                int height = 10;
                if ((i + value) % 12 == 0)
                {
                    height = 28;
                    pen.Brush = Brushes.Black;
                    dc.DrawText(new FormattedText(MediaTools.GetTimeText((i + value) * Weight, FrameRate), CultureInfo.GetCultureInfo("en-us"),
                        FlowDirection.LeftToRight,
                        new Typeface("/Delight;component/Resources/Fonts/#Helvetica"),
                        10, Brushes.Black),
                        new Point(startPoint + i * _displaySize + sizeOffset + 4, 20));

                    guidelines.GuidelinesY.Add(35.5 - height);
                    //DrawLine(dc, pen, new Point(startPoint + i * _displaySize + sizeOffset, 35 - height), new Point(startPoint + i * _displaySize + sizeOffset + 3, 35 - height));
                }
                else if ((i + value) % 4 == 0)
                {
                    height = 20;
                    pen.Brush = Brushes.Black;
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
