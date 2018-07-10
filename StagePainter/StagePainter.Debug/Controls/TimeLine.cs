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

using StagePainter.Core.Common;
using StagePainter.Core.Extension;
using StagePainter.Debug.Manager;

namespace StagePainter.Debug.Controls
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

            this.SizeChanged += (s, e) =>
            {
                SetItemsValue();
            };

            scrollBar.ViewportSize = 100;
            scrollBar.Scroll += ScrollBar_Scroll;
            gridDrag.MouseDown += GridDrag_MouseDown;
            AddTrack();
            AddTrack();
            AddTrack();
            AddTrack();
        }

        private void ScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            _offset = scrollBar.Value;
            this.InvalidateVisual();
            SetItemsValue();
        }

        public void AddTrack()
        {
            Grid grid = new Grid()
            {
                Height = 50,
                Background = Brushes.Transparent,
                AllowDrop = true,
            };
            grid.Children.Add(new Border()
            {
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

        TimeLineItemDesign tempRect = null;
        int i = 0;

        private void Grid_DragLeave(object sender, DragEventArgs e)
        {
            if (tempRect != null)
            {
                ((Grid)sender).Children.Remove(tempRect);
                tempRect = null;

                MainWindow mw = (MainWindow)(((Grid)this.Parent).Parent);
                mw.Title = i++.ToString();
            }
        }

        private void Grid_DragEnter(object sender, DragEventArgs e)
        {
            if (tempRect == null)
            {
                tempRect = new TimeLineItemDesign()
                {
                    Width = ((TestDataPack)e.Data.GetData(typeof(TestDataPack))).Size * _realSize,
                    IsHitTestVisible = false,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Tag = ((TestDataPack)e.Data.GetData(typeof(TestDataPack)))
                };

                tempRect.dragLeft.MouseDown += DragLeft;
                tempRect.dragRight.MouseDown += DragRight;
                tempRect.dragMove.MouseDown += DragMove;
                tempRect.MouseLeave += DragToAnother;
                ((Grid)sender).Children.Add(tempRect);
            }

            Point relativePoint = this.TransformToAncestor((Visual)this.Parent)
                                      .Transform(new Point(0, 0));

            PresentationSource source = PresentationSource.FromVisual(this);
            Point locationFromScreen = this.PointToScreen(new Point(0, 0));

            var point = source.CompositionTarget.TransformFromDevice.Transform(locationFromScreen);

            double left = MouseManager.MousePosition.X - point.X;

            int value = (int)((left + _offset) / _realSize);
            tempRect.Margin = new Thickness((int)(left / _realSize) * _realSize, 0, 0, 0);

            (tempRect.Tag as TestDataPack).Offset = value;
        }

        private void DragToAnother(object sender, MouseEventArgs e)
        {
            //DragDrop.DoDragDrop(this, sender, DragDropEffects.Move);
            //Grid grid = (((Control)sender).Parent) as Grid;
            //grid.Children.Remove(sender as UIElement);
        }

        #region [  TrackItem Drag & Resize  ]

        int firstX;
        double firstOffset;
        double firstSize;

        private void DragMove(object sender, MouseButtonEventArgs e)
        {
            var root = ((FrameworkElement)VisualTreeHelper.GetParent((Rectangle)sender)).Parent;
            if (root is TimeLineItemDesign ti)
            {
                TestDataPack pack = ti.Tag as TestDataPack;
                firstX = MouseManager.MousePosition.X;
                firstOffset = ti.Margin.Left;
                Thread thr = new Thread(() =>
                {
                    while (MouseManager.IsMouseDown)
                    {
                        Thread.Sleep(1);
                        Dispatcher.Invoke(() =>
                        {
                            int mouseOffset = MouseManager.MousePosition.X - firstX;
                            int packOffset = (int)((firstOffset + mouseOffset + _offset) / _realSize);

                            if (packOffset < 0)
                                packOffset = 0;

                            ti.Margin = new Thickness((packOffset * _realSize) - _offset, ti.Margin.Top, ti.Margin.Right, ti.Margin.Bottom);
                            pack.Offset = packOffset;
                        });
                    }

                    firstX = 0;
                    firstOffset = 0;
                    firstSize = 0;
                    Dispatcher.Invoke(() =>
                    {
                        this.InvalidateVisual();
                        SetItemsValue();
                    });
                });

                thr.Start();
            }
        }

        private void DragRight(object sender, MouseButtonEventArgs e)
        {
            var root = ((FrameworkElement)VisualTreeHelper.GetParent((Rectangle)sender)).Parent;
            if (root is TimeLineItemDesign ti)
            {
                TestDataPack pack = ti.Tag as TestDataPack;
                firstX = MouseManager.MousePosition.X;
                firstSize = ti.Width;
                Thread thr = new Thread(() =>
                {
                    while (MouseManager.IsMouseDown)
                    {
                        Thread.Sleep(1);
                        Dispatcher.Invoke(() =>
                        {
                            int mouseOffset = MouseManager.MousePosition.X - firstX;
                            int packWidth = (int)((firstSize + mouseOffset) / _realSize);
                            if (packWidth < 1)
                                packWidth = 1;
                            ti.Width = (packWidth * _realSize);
                            pack.Size = packWidth;
                        });
                    }
                    firstX = 0;
                    firstOffset = 0;
                    firstSize = 0;
                    Dispatcher.Invoke(() => SetItemsValue());
                });

                thr.Start();
            }
        }

        private void DragLeft(object sender, MouseButtonEventArgs e)
        {
            var root = ((FrameworkElement)VisualTreeHelper.GetParent((Rectangle)sender)).Parent;
            if (root is TimeLineItemDesign ti)
            {
                TestDataPack pack = ti.Tag as TestDataPack;
                firstX = MouseManager.MousePosition.X;
                firstOffset = ti.Margin.Left;
                firstSize = ti.Width;
                Thread thr = new Thread(() =>
                {
                    while (MouseManager.IsMouseDown)
                    {
                        Thread.Sleep(1);
                        Dispatcher.Invoke(() =>
                        {
                            int mouseOffset = MouseManager.MousePosition.X - firstX;

                            int packWidth = (int)((firstSize - mouseOffset) / _realSize);
                            int packOffset = (int)((firstOffset + mouseOffset + _offset) / _realSize);

                            int overWidth = 0, overOffset = 0;

                            if (packOffset < -(scrollBar.Maximum * _realSize))
                            {
                                overOffset = packOffset;
                                packOffset = 0;
                            }
                            else if (packWidth < 1)
                            {
                                overWidth = packWidth;
                                packWidth = 1;
                            }

                            ti.Margin = new Thickness((packOffset * _realSize) + (overWidth * _realSize) - _offset, ti.Margin.Top, ti.Margin.Right, ti.Margin.Bottom);
                            pack.Offset = packOffset + overWidth;

                            ti.Width = (packWidth * _realSize) + (overOffset * _realSize);
                            pack.Size = packWidth - overOffset;
                        });
                    }
                    firstX = 0;
                    firstOffset = 0;
                    firstSize = 0;
                    Dispatcher.Invoke(() => SetItemsValue());
                });

                thr.Start();
            }
        }

        #endregion
        private void Grid_Drop(object sender, DragEventArgs e)
        {
            tempRect.IsHitTestVisible = true;
            tempRect = null;
            SetItemsValue();
        }

        #region [  Dependency Property  ]

        private int _value;

        public int Value
        {
            get => _value;
            set
            {
                _value = value;
                SetItemsValue();
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

                SetItemsValue();
            }
        }

        #endregion

        #region [  Property & Variable  ]

        public FrameRate FrameRate { get; private set; } = FrameRate._60FPS;

        public double MaxItemSize = 30;

        public double ItemSize = 1;

        private int Weight => GetWeight(Ratio);

        private double _realSize => ItemSize * Ratio;

        private double _displaySize => _realSize * Weight;

        private double _offset { get; set; }

        #endregion

        private void SetItemsValue()
        {
            IEnumerable<TimeLineItemDesign> rects = tracks.Select(i => i.Children
                                        .Cast<UIElement>()
                                        .Where(j => j.GetType() == typeof(TimeLineItemDesign))
                                        .Cast<TimeLineItemDesign>())
                                        .SelectMany(k => k);
            if (rects.Count() != 0)
            {
                double max = rects.Select(i => (TestDataPack)i.Tag)
                              .Max(i => (i.Offset * _realSize) + (i.Size * _realSize));

                max -= ActualWidth;

                if (max != 0)
                {
                    scrollBar.Maximum = max;
                    if (scrollBar.Maximum < _offset)
                    {
                        _offset = scrollBar.Maximum;
                        scrollBar.Value = scrollBar.Maximum;
                    }
                    else
                    {
                        _offset = scrollBar.Value;
                    }
                }
                else
                {
                    _offset = 0;
                }
                scrollBar.Visibility = (max != 0) ? Visibility.Visible : Visibility.Hidden;

                foreach (TimeLineItemDesign rect in rects)
                {
                    TestDataPack pack = rect.Tag as TestDataPack;

                    double offset = pack.Offset * _realSize,
                           width = pack.Size * _realSize;

                    rect.Margin = new Thickness(offset - _offset, 0, 0, 0);
                    rect.Width = width;
                }
            }

            trackSlider.Margin = new Thickness((Value * _realSize) - 0.5 - _offset, 0, 0, 0);
        }

        private void GridDrag_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Thread thr = new Thread(() =>
            {
                while (MouseManager.IsMouseDown)
                {
                    Thread.Sleep(1);
                    Dispatcher.Invoke(() =>
                    {
                        Point relativePoint = this.TransformToAncestor((Visual)this.Parent)
                                      .Transform(new Point(0, 0));

                        PresentationSource source = PresentationSource.FromVisual(this);
                        Point locationFromScreen = this.PointToScreen(new Point(0, 0));

                        var point = source.CompositionTarget.TransformFromDevice.Transform(locationFromScreen);

                        double left = MouseManager.MousePosition.X - point.X + _offset;
                        if (left < 0)
                            left = 0;

                        MainWindow mw = (MainWindow)(((Grid)this.Parent).Parent);
                        mw.Title = GetTimeText((int)(left / _realSize));

                        Value = (int)(left / _realSize);
                    });
                }
            });
            thr.SetApartmentState(ApartmentState.STA);
            thr.Start();
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
                    dc.DrawText(new FormattedText(GetTimeText((i + value) * Weight), CultureInfo.GetCultureInfo("en-us"),
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
        public string GetTimeText(int value)
        {
            int frameRate = (int)FrameRate.GetEnumAttribute<DefaultValueAttribute>().Value;
            int frame = value % frameRate;
            int second = value / frameRate;
            int minute = second / 60;
            int hour = minute / 60;

            second -= minute * 60;
            minute -= hour * 60;

            return $"{hour.ToString("00")}:{minute.ToString("00")}:{second.ToString("00")}.{frame}";
        }
    }
}
