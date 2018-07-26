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
using Delight.Core.Common;
using Delight.Core.Extension;
using Delight.Components.Medias;
using Delight.Components.Common;
using Delight.Components;
using Delight.Extensions;

namespace Delight.Controls
{
    [TemplatePart(Name = "trackSlider", Type = typeof(Rectangle))]
    [TemplatePart(Name = "trackPanel", Type = typeof(StackPanel))]
    [TemplatePart(Name = "scrollBar", Type = typeof(ScrollBar))]
    [TemplatePart(Name = "gridDrag", Type = typeof(Grid))]
    public class TimeLine : Control
    {
        public TimeLine()
        {
            this.MouseWheel += TimeLine_MouseWheel;

            this.Style = FindResource("TimeLineStyle") as Style;
        }

        public event EventHandler FrameChanged;
        public event EventHandler FrameMouseChanged;

        public Rectangle trackSlider;
        public StackPanel trackPanel;
        public ScrollBar scrollBar;
        public Grid gridDrag;

        public List<Grid> tracks = new List<Grid>();

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            trackSlider = GetTemplateChild("trackSlider") as Rectangle;
            trackPanel = GetTemplateChild("trackPanel") as StackPanel;
            scrollBar = GetTemplateChild("scrollBar") as ScrollBar;
            gridDrag = GetTemplateChild("gridDrag") as Grid;

            this.SizeChanged += (s, e) => ResetItemOffset();

            scrollBar.ViewportSize = 100;
            scrollBar.Scroll += ScrollBar_Scroll;
            gridDrag.MouseLeftButtonDown += GridDrag_MouseLeftButtonDown;
            gridDrag.MouseMove += GridDrag_MouseMove;
            gridDrag.MouseLeftButtonUp += GridDrag_MouseLeftButtonUp;
            AddTrack();
            AddTrack();
            AddTrack();
            AddTrack();
        }

        public void AddTrack()
        {
            Grid grid = new Grid()
            {
                Background = Brushes.Transparent,
                AllowDrop = true,
            };
            grid.Children.Add(new Border()
            {
                Height = 50,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1, 0, 1, 1),
                IsHitTestVisible = false,
            });

            grid.DragEnter += Grid_DragEnter;
            grid.DragOver += Grid_DragEnter;
            grid.DragLeave += Grid_DragLeave;
            grid.Drop += Grid_Drop;

            tracks.Add(grid);
            trackPanel.Children.Add(grid);
        }

        TrackItem tempItem = null;

        private void ScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            _offset = scrollBar.Value;
            this.InvalidateVisual();
            ResetItemOffset();
        }

        private void Grid_DragLeave(object sender, DragEventArgs e)
        {
            if (tempItem != null)
            {
                ((Grid)sender).Children.Remove(tempItem);
                tempItem = null;

                MainWindow mw = Application.Current.MainWindow as MainWindow;
            }
        }

        private void Grid_DragEnter(object sender, DragEventArgs e)
        {
            if (tempItem == null)
            {
                var comp = e.Data.GetData(e.Data.GetFormats()[0]) as StageComponent;
                var frame = MediaTools.TimeSpanToFrame(comp.Time, FrameRate);

                tempItem = new TrackItem()
                {
                    Width = frame * _realSize,
                    FrameWidth = frame,
                    MaxFrame = frame,
                    IsHitTestVisible = false,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Text = comp.Identifier,
                    Thumbnail = comp.Thumbnail,
                };

                tempItem.DragLeftMouseLeftButtonDown += TrackItem_HoldLeft;
                tempItem.DragRightMouseLeftButtonDown += TrackItem_HoldRight;



                ((Grid)sender).Children.Add(tempItem);
            }

            Point relativePoint = this.TransformToAncestor((Visual)this.Parent)
                                      .Transform(new Point(0, 0));

            PresentationSource source = PresentationSource.FromVisual(this);
            Point locationFromScreen = this.PointToScreen(new Point(0, 0));

            var point = source.CompositionTarget.TransformFromDevice.Transform(locationFromScreen);

            double left = MouseManager.MousePosition.X - point.X;

            int value = (int)((left + _offset) / _realSize);
            tempItem.Offset = value;
            tempItem.Margin = new Thickness((int)(left / _realSize) * _realSize, 0, 0, 0);
        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            tempItem.IsHitTestVisible = true;
            tempItem = null;
            ResetItemOffset();
        }

        #region [  TrackItem Drag & Resize  ]

        int firstX; // 처음 X 좌표 위치 (드래그, 왼쪽으로 늘리기, 오른쪽으로 늘리기에서 사용)
        double firstOffset; // 처음 Offset (드래그, 왼쪽으로 늘리기에서 사용)
        double firstSize; // 처음 사이즈 (왼쪽으로 늘리기, 오른쪽으로 늘리기에서 사용)

        /// <summary>
        /// TrackItem을 잡고 끌때 발생하는 이벤트입니다.
        /// </summary>
        private void TrackItem_DragMove(object sender, MouseButtonEventArgs e)
        {

            // 현재 전달된 sender의 템플릿 루트가 TrackItem이라면 ti라는 변수로 받는다.
            if (((Rectangle)sender).TemplatedParent is TrackItem item)
            {

                // 최초 X 좌표 위치는 절대값으로 받는다.
                firstX = MouseManager.MousePosition.X;
                // 최초 Offset은 TrackItem의 왼쪽 마진으로 받는다.
                firstOffset = item.Margin.Left;

                Thread thr = new Thread(() =>
                {
                    // 마우스가 눌리고 있는 중에 계속 반복
                    while (MouseManager.IsMouseDown)
                    {
                        Thread.Sleep(1);
                        // TimeLine의 메인 쓰레드에 접근하기 위해서 컨트롤 소유자인 Dispatcher에 접근
                        Dispatcher.Invoke(() =>
                        {
                            // 내가 추가해야 할 오프셋 값 => 현재 X 좌표 - 최초 X 좌표
                            int addOffset = MouseManager.MousePosition.X - firstX;
                            // 실제 오프셋 => ('아이템의 최초 오프셋' + '추가해야 할 오프셋 값' + '슬라이더의 오프셋 값') / '실제로 보여지는 한 프레임의 크기'
                            // 이렇게 계산하는 이유 : 정확히 프레임 단위로 이동하기 위해서 값을 정규화 시키기 위함.
                            int rawOffset = (int)((firstOffset + addOffset + _offset) / _realSize);

                            // 만약 packOffset이 0보다 작다면 0으로 무조건 맟추기
                            if (rawOffset < 0)
                                rawOffset = 0;

                            // 나머지는 모두 같지만 왼쪽 마진값만 실제 값을 곱해서 위치 시킴.
                            item.Margin = ChangeLeftMargin((rawOffset * _realSize) - _offset, item.Margin);
                            // 내부적으로는 실제 오프셋을 저장하고 있음
                            item.Offset = rawOffset;
                        });
                    }

                    // 사용했던 변수들 초기화
                    firstX = 0;
                    firstOffset = 0;

                    Dispatcher.Invoke(() =>
                    {
                        // 컨트롤 범위 무효화 (다시 그리기)
                        this.InvalidateVisual();
                        // 아이템 위치 재설정
                        ResetItemOffset();
                    });
                });

                thr.Start();
            }
        }

        /// <summary>
        /// TrackItem의 오른쪽 끝 부분을 잡고 늘릴때 발생하는 이벤트입니다.
        /// </summary>
        private void TrackItem_HoldRight(object sender, MouseButtonEventArgs e)
        {
            // 현재 전달된 sender의 템플릿 루트가 TrackItem이라면 ti라는 변수로 받는다.
            if (((Rectangle)sender).TemplatedParent is TrackItem item)
            {
                // 최초 X 좌표 위치는 절대값으로 받는다.
                firstX = MouseManager.MousePosition.X;
                // 최초 사이즈는 TrackItem의 실제 Width로 받는다.
                firstSize = item.Width;

                Thread thr = new Thread(() =>
                {
                    // 마우스가 눌리고 있는 중에 계속 반복
                    while (MouseManager.IsMouseDown)
                    {
                        Thread.Sleep(1);
                        // TimeLine의 메인 쓰레드에 접근하기 위해서 컨트롤 소유자인 Dispatcher에 접근
                        Dispatcher.Invoke(() =>
                        {
                            // 내가 추가해야 할 오프셋 값 => 현재 X 좌표 - 최초 X 좌표
                            int addOffset = MouseManager.MousePosition.X - firstX;
                            // 실제 프레임 너비 => (최초 너비 + 추가할 오프셋)  / 실제 한 프레임 크기
                            int rawWidth = (int)((firstSize + addOffset) / _realSize);
                            // 만약 프레임 너비가 1보다 작다면 1로 설정
                            if (rawWidth < 1)
                                rawWidth = 1;

                            #region [  다시 짜야할 부분  ]
                            //int maximum = item.MaxFrame - item.StartOffset;

                            //if (rawWidth > maximum)
                            //{
                            //    rawWidth = maximum;
                            //}

                            //item.EndOffset = item.MaxFrame - rawWidth;
                            #endregion

                            // TrackItem의 길이 => 프레임 너비 * 한 프레임의 크기
                            item.Width = (rawWidth * _realSize);
                            // 내부 프레임 길이에는 프레임 너비 그대로 저장
                            item.FrameWidth = rawWidth;
                        });
                    }

                    // 사용한 변수 초기화
                    firstX = 0;
                    firstSize = 0;

                    Dispatcher.Invoke(() =>
                    {
                        // 컨트롤 범위 무효화 (다시 그리기)
                        this.InvalidateVisual();
                        // 아이템 위치 재설정
                        ResetItemOffset();
                    });
                });

                thr.Start();
            }
        }

        /// <summary>
        /// TrackItem의 왼쪽 끝 부분을 잡고 늘릴때 발생하는 이벤트입니다.
        /// </summary>
        private void TrackItem_HoldLeft(object sender, MouseButtonEventArgs e)
        {
            // 현재 전달된 sender의 템플릿 루트가 TrackItem이라면 ti라는 변수로 받는다.
            if (((Rectangle)sender).TemplatedParent is TrackItem item)
            {
                // 최초 X 좌표 위치는 절대값으로 받는다.
                firstX = MouseManager.MousePosition.X;
                // 최초 Offset은 TrackItem의 왼쪽 마진으로 받는다.
                firstOffset = item.Margin.Left;
                // 최초 사이즈는 TrackItem의 실제 Width로 받는다.
                firstSize = item.Width;

                Thread thr = new Thread(() =>
                {
                    while (MouseManager.IsMouseDown)
                    {
                        Thread.Sleep(1);
                        Dispatcher.Invoke(() =>
                        {
                            // 내가 추가해야 할 오프셋 값 => 현재 X 좌표 - 최초 X 좌표
                            int addOffset = MouseManager.MousePosition.X - firstX;

                            // 실제 프레임 너비 => (최초 너비 + 추가할 오프셋)  / 실제 한 프레임 크기
                            int rawWidth = (int)((firstSize - addOffset) / _realSize);
                            // 실제 오프셋 => ( 최초 오프셋 + 추가할 오프셋 + 슬라이더의 오프셋) / 실제 한 프레임 크기
                            int rawOffset = (int)((firstOffset + addOffset + _offset) / _realSize);

                            int overWidth = 0, overOffset = 0;

                            // 만약 오프셋이 0보다 뒤로 갈때
                            if ((rawOffset * _realSize) < 0)
                            {
                                // 넘게 되는 값을 overOffset으로 처리
                                overOffset = rawOffset;
                                // 실제 처리에 쓸 값은 0으로 고정
                                rawOffset = 0;
                            }
                            else if (rawWidth < 1)
                            {
                                // 넘게 되는 너비를 overWidth로 처리
                                overWidth = rawWidth;
                                // 실제 처리에 쓸 최소 너비는 1로 고정
                                rawWidth = 1;
                            }

                            #region [  다시 짜야할 부분  ]

                            //int maximum = item.MaxFrame - item.EndOffset;

                            //if (rawWidth >= maximum)
                            //{
                            //    int over = rawWidth - maximum;

                            //    rawWidth = maximum;
                            //    var current = (int)((firstOffset + _offset) / _realSize);
                            //    rawOffset = maximum - (maximum - current);

                            //    Console.WriteLine(maximum + " :: " + current + " :: " + rawOffset);

                            //}

                            //item.StartOffset = item.MaxFrame - rawWidth;

                            #endregion

                            // 실제 왼쪽 마진 값은 원래 왼쪽 값과 초과되는 너비를 추가해준다. (-슬라이더 오프셋 값)
                            item.Margin = ChangeLeftMargin((rawOffset + overWidth) * _realSize - _offset, item.Margin);
                            item.Offset = rawOffset + overWidth;

                            // 너비는 원래 너비와 초과된 오프셋을 추가해준다.
                            item.Width = (rawWidth + overOffset) * _realSize;
                            item.FrameWidth = rawWidth + overOffset;

                        });
                    }
                    // 사용한 변수 초기화
                    firstX = 0;
                    firstOffset = 0;
                    firstSize = 0;

                    Dispatcher.Invoke(() =>
                    {
                        this.InvalidateVisual();
                        ResetItemOffset();
                    });
                });

                thr.Start();
            }
        }

        private Thickness ChangeLeftMargin(double left, Thickness thickness)
        {
            return new Thickness(left, thickness.Top, thickness.Right, thickness.Bottom);
        }

        #endregion

        #region [  Dependency Property  ]

        private int _value;

        public int Frame
        {
            get => _value;
            set
            {
                bool same = _value == value;

                _value = value;

                if (!same)
                    FrameChanged?.Invoke(this, new EventArgs());

                ResetItemOffset();
            }
        }

        public static DependencyProperty RatioProperty = DependencyProperty.Register(nameof(Ratio),
            typeof(double),
            typeof(TimeLine),
            new FrameworkPropertyMetadata(0.1, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        private const double MaxRatio = 12.8;

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

                scrollBar.ViewportSize = value * 100;

                ResetItemOffset();
            }
        }

        #endregion

        #region [  Property & Variable  ]

        public FrameRate FrameRate { get; private set; } = FrameRate._60FPS;

        public double MaxItemSize = 30;

        public double ItemSize = 0.5;

        private int Weight => GetWeight(Ratio);

        private double _realSize => ItemSize * Ratio;

        private double _displaySize => _realSize * Weight;

        private double _offset { get; set; }

        #endregion

        private void ResetItemOffset()
        {
            this.ApplyTemplate();
            IEnumerable<TrackItem> items = tracks.Select(i => i.Children
                .Cast<UIElement>()
                .Where(j => j.GetType() == typeof(TrackItem))
                .Cast<TrackItem>())
                .SelectMany(k => k);
            if (items.Count() != 0)
            {
                double max = items.Max(i => (i.Offset * _realSize) + (i.FrameWidth * _realSize));

                max -= ActualWidth;

                if (max != 0)
                {
                    scrollBar.Maximum = max;
                    if (scrollBar.Maximum < _offset)
                        _offset = scrollBar.Maximum;
                    else
                        _offset = scrollBar.Value;
                }
                else
                {
                    _offset = 0;
                }
                scrollBar.Visibility = (max != 0) ? Visibility.Visible : Visibility.Hidden;

                foreach (TrackItem itm in items)
                {
                    double offset = itm.Offset * _realSize,
                           width = itm.FrameWidth * _realSize;
                    itm.Margin = new Thickness(offset - _offset, 0, 0, 0);
                    itm.Width = width;
                }
            }
            trackSlider.Margin = new Thickness((Frame * _realSize) - 0.5 - _offset, 0, 0, 0);
        }

        bool captured = false;
        UIElement source;
        double absLeft, relLeft;

        private void GridDrag_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            source = (UIElement)sender;
            Mouse.Capture(source);
            captured = true;
            relLeft = e.GetPosition(this).X;
            absLeft = e.GetPosition(this).X;

            SetSliderLeft();
        }

        private void GridDrag_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            captured = false;
        }

        private void GridDrag_MouseMove(object sender, MouseEventArgs e)
        {
            if (captured)
            {
                double x = e.GetPosition(this).X;
                relLeft += x - absLeft;

                SetSliderLeft();

                absLeft = x;
            }
        }

        public void SetSliderLeft()
        {
            double left = relLeft + _offset;
            if (left < 0)
                left = 0;

            var frame = (int)(left / _realSize);

            if (frame != Frame)
            {
                trackSlider.Margin = new Thickness((frame * _realSize) - 0.5 - _offset, 0, 0, 0);
                Frame = frame;
                FrameMouseChanged?.Invoke(this, new EventArgs());
            }
        }

        #region [  Get/Set Left/Top  ]

        public double GetLeft(object element)
        {
            if (element is FrameworkElement ui)
                return ui.Margin.Left;

            return double.MinValue;
        }

        public double GetTop(object element)
        {
            if (element is FrameworkElement ui)
                return ui.Margin.Top;

            return double.MinValue;
        }

        public void SetLeft(object element, double left)
        {
            if (element is FrameworkElement ui)
                ui.Margin = new Thickness(left, ui.Margin.Top, ui.Margin.Right, ui.Margin.Bottom);
        }

        public void SetTop(object element, double top)
        {
            if (element is FrameworkElement ui)
                ui.Margin = new Thickness(ui.Margin.Left, top, ui.Margin.Right, ui.Margin.Bottom);
        }

        #endregion
        
        public IEnumerable<TrackItem> GetTrackItems(int frame)
        {
            IEnumerable<TrackItem> items = tracks.Select(i => i.Children
                .Cast<UIElement>()
                .Where(j => j.GetType() == typeof(TrackItem))
                .Cast<TrackItem>())
                .SelectMany(k => k)
                .Where(i => IsInRange(frame, i));

            return items;
        }

        public bool IsInRange(int frame, TrackItem itm)
        {
            return itm.Offset <= frame && itm.FrameWidth + itm.Offset >= frame;
        }

        protected override void OnRender(DrawingContext dc)
        {
            dc.DrawRectangle(Brushes.Transparent, null, new Rect(0, 0, this.ActualWidth, this.ActualHeight));
            DrawHelperLine(dc);
            base.OnRender(dc);
        }

        private void TimeLine_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if (e.Delta > 0)
                    Ratio += GetIncrement(Ratio);
                else
                    Ratio -= GetIncrement(Ratio);
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

            int weight = (int)Math.Pow(2, 7);

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
            double sizeOffset = _offset == 0 ? 0 : (_displaySize - (_offset % _displaySize)) - _displaySize;
            int value = (int)(_offset / _displaySize);

            for (int i = 0; i <= ActualWidth / _displaySize; i++)
            {
                var pen = new Pen(Brushes.Gray, 1);

                double halfPenWidth = pen.Thickness / 2;

                GuidelineSet guidelines = new GuidelineSet();

                guidelines.GuidelinesX.Add((i * _displaySize) + sizeOffset + halfPenWidth);
                guidelines.GuidelinesX.Add((i * _displaySize) + sizeOffset + _displaySize + halfPenWidth);

                dc.PushGuidelineSet(guidelines);
                int height = 10;
                if ((i + value) % 12 == 0)
                {
                    height = 28;
                    pen.Brush = Brushes.Black;
                    dc.DrawText(new FormattedText(MediaTools.GetTimeText((i + value) * Weight, FrameRate), CultureInfo.GetCultureInfo("en-us"),
                        FlowDirection.LeftToRight,
                        new Typeface("Arial"),
                        10, Brushes.Black),
                        new Point(i * _displaySize + sizeOffset + 4, 2));

                    guidelines.GuidelinesY.Add(35.5 - height);
                    dc.DrawLine(pen, new Point(i * _displaySize + sizeOffset, 35 - height), new Point(i * _displaySize + sizeOffset + 3, 35 - height));
                }
                else if ((i + value) % 4 == 0)
                {
                    height = 20;
                    pen.Brush = Brushes.Black;
                }
                dc.DrawLine(pen, new Point(i * _displaySize + sizeOffset, 35 - height), new Point(i * _displaySize + sizeOffset, 35));
                dc.Pop();
            }
        }
    }
}
