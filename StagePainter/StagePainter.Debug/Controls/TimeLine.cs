using StagePainter.Core.Common;
using StagePainter.Core.Extension;
using StagePainter.Debug.Manager;
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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace StagePainter.Debug.Controls
{ 
    [TemplatePart(Name = "trackSlider", Type = typeof(Rectangle))]
    [TemplatePart(Name = "trackPanel", Type = typeof(StackPanel))]
    public class TimeLine : Control
    {
        public TimeLine()
        {
            this.MouseWheel += TimeLine_MouseWheel;
            this.MouseDown += TimeLine_MouseDown;
            
            this.Style = FindResource("TimeLineStyle") as Style;

            //int value = (int)FrameRate._24FPS.GetEnumAttribute<DefaultValueAttribute>().Value;
            //MessageBox.Show(value.ToString());
        }

        public Rectangle trackSlider;
        public StackPanel trackPanel;

        public List<Grid> tracks = new List<Grid>();

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            trackSlider = GetTemplateChild("trackSlider") as Rectangle;
            trackPanel = GetTemplateChild("trackPanel") as StackPanel;

            AddTrack();
            AddTrack();
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
                BorderThickness = new Thickness(1,0,1,1),
                IsHitTestVisible = false,
            });

            grid.DragEnter += Grid_DragEnter;
            grid.DragOver += Grid_DragEnter;
            grid.DragLeave += Grid_DragLeave;
            grid.Drop += Grid_Drop;

            tracks.Add(grid);
            trackPanel.Children.Add(grid);
        }
        
        Rectangle tempRect = null;

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
                tempRect = new Rectangle()
                {
                    Width = ((TestDataPack)e.Data.GetData(typeof(TestDataPack))).Size * _realSize,
                    Stroke = Brushes.Black,
                    Fill = Brushes.Green,
                    StrokeThickness = 1,
                    IsHitTestVisible = false,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Tag = ((TestDataPack)e.Data.GetData(typeof(TestDataPack)))
                };
                ((Grid)sender).Children.Add(tempRect);
            }
                

            Point relativePoint = this.TransformToAncestor((Visual)this.Parent)
                                      .Transform(new Point(0, 0));

            PresentationSource source = PresentationSource.FromVisual(this);
            Point locationFromScreen = this.PointToScreen(new Point(0, 0));

            var point = source.CompositionTarget.TransformFromDevice.Transform(locationFromScreen);

            double left = MouseManager.MousePosition.X - point.X;

            int value = (int)(left / _realSize);
            tempRect.Margin = new Thickness(value * _realSize, 0, 0, 0);

            (tempRect.Tag as TestDataPack).Offset = value;
        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            tempRect = null;
            TestDataPack pack = (TestDataPack)e.Data.GetData(typeof(TestDataPack));
            MessageBox.Show(pack.Name);
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
            new FrameworkPropertyMetadata(12.8, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

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
                SetItemsValue();
                
            }
        }

        #endregion

        #region [  Property & Variable  ]

        public FrameRate FrameRate { get; private set; } = FrameRate._24FPS;

        public double MaxItemSize = 30;

        public double ItemSize = 1;

        private int Weight => GetWeight(Ratio);

        private double _realSize => ItemSize * Ratio;

        private double _displaySize => _realSize * Weight;

        #endregion

        private void SetItemsValue()
        {
            trackSlider.Margin = new Thickness((Value * _realSize) - 0.5, 0, 0, 0);
            foreach(Grid g in tracks)
            {
                foreach(Rectangle rect in g.Children.Cast<UIElement>().Where(i => i.GetType() == typeof(Rectangle)))
                {
                    TestDataPack pack = rect.Tag as TestDataPack;
                    rect.Margin = new Thickness(pack.Offset * _realSize, 0, 0, 0);
                    rect.Width = pack.Size * _realSize;
                }
            }
        }

        private void TimeLine_MouseDown(object sender, MouseButtonEventArgs e)
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

                        double left = MouseManager.MousePosition.X - point.X;
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
            if (!Keyboard.IsKeyDown(Key.LeftAlt))
                return;

            if (e.Delta > 0)
                Ratio += GetIncrement(Ratio);
            else
                Ratio -= GetIncrement(Ratio);
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
            for (int i = 0; i <= ActualWidth / _displaySize; i++)
            {
                var pen = new Pen(Brushes.Gray, 1);

                double halfPenWidth = pen.Thickness / 2;

                GuidelineSet guidelines = new GuidelineSet();

                guidelines.GuidelinesX.Add((i * _displaySize) + halfPenWidth);
                guidelines.GuidelinesX.Add((i * _displaySize) + _displaySize + halfPenWidth);
                //guidelines.GuidelinesY.Add(65.5);

                dc.PushGuidelineSet(guidelines);
                int height = 10;
                if (i % 12 == 0)
                {
                    height = 28;
                    pen.Brush = Brushes.Black;
                    dc.DrawText(new FormattedText(GetTimeText(i * Weight), CultureInfo.GetCultureInfo("en-us"),
                        FlowDirection.LeftToRight,
                        new Typeface("Arial"),
                        10, Brushes.Black),
                        new Point(i * _displaySize + 4, 2));

                    guidelines.GuidelinesY.Add(35.5 - height);
                    dc.DrawLine(pen, new Point(i * _displaySize, 35 - height), new Point(i * _displaySize + 3, 35 - height));
                }
                else if (i % 4 == 0)
                {
                    height = 20;
                    pen.Brush = Brushes.Black;
                }
                dc.DrawLine(pen, new Point(i * _displaySize, 35 - height), new Point(i * _displaySize, 35));

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
