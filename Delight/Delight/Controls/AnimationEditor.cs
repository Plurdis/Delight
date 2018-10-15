using Delight.Components.Common;
using Delight.Extensions;
using System;
using System.Collections.Generic;
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

namespace Delight.Controls
{
    [TemplatePart(Name = "positioner", Type = typeof(Rectangle))]
    [TemplatePart(Name = "dragRange", Type = typeof(Grid))]
    [TemplatePart(Name = "scrollBar", Type = typeof(ScrollBar))]
    [TemplatePart(Name = "itemName", Type = typeof(TextBlock))]
    public class AnimationEditor : Control
    {
        public AnimationEditor()
        {
            this.Style = FindResource("AnimationEditorStyle") as Style;

            ApplyTemplate();
        }

        public void SetTimeLine(TimeLine timeLine)
        {
            this.TimeLine = timeLine;
            TimeLine.FrameChanged += TimeLine_FrameChanged;
        }

        private void TimeLine_FrameChanged(object sender, EventArgs e)
        {
            SetPositionerToPosition();
        }

        #region [  전역 변수  ]

        public TimeLine TimeLine { get; private set; }

        public TrackItem SelectedTrackItem { get; private set; }

        Rectangle positioner;
        ScrollBar scrollBar;
        Grid dragRange;
        TextBlock itemName;

        public const double ItemSize = 0.25;

        public double Offset => 5; // scrollBar.Value

        private int Weight => GetWeight(Ratio);

        private double _realSize => ItemSize * Ratio;

        private double _displaySize => _realSize * Weight;


        public double MaxValue { get; set; }

        public int MaxFrame { get; set; }

        private const double MaxRatio = 25.6;

        #endregion

        #region [  DependencyProperty  ]

        public static DependencyProperty RatioProperty = DependencyProperty.Register(nameof(Ratio),
            typeof(double),
            typeof(AnimationEditor),
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

                //visualTracks.Children.Cast<Track>().ForEach(i => i.RelocationTrackItems());

                //ResetItemOffset();
            }
        }

        #endregion

        private void Track_ItemsMaxWidthChanged(object sender, EventArgs e)
        {
            if (SelectedTrackItem == null)
                return;

            MaxFrame = SelectedTrackItem.Offset + SelectedTrackItem.FrameWidth;
            MaxValue = (SelectedTrackItem.Offset + SelectedTrackItem.FrameWidth) * _realSize;

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

        private void SetPositionerToPosition()
        {
            int position = 0;
            if (TimeLine != null)
                position = TimeLine.Position;

            positioner.Margin = new Thickness(position * _realSize - 0.5 - Offset, 0, 0, 0);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            positioner = GetTemplateChild("positioner") as Rectangle;
            scrollBar = GetTemplateChild("scrollBar") as ScrollBar;
            dragRange = GetTemplateChild("dragRange") as Grid;
            itemName = GetTemplateChild("itemName") as TextBlock;

            dragRange.MouseLeftButtonDown += DragRange_MouseLeftButtonDown;
            dragRange.MouseLeftButtonUp += DragRange_MouseLeftButtonUp;
            dragRange.MouseMove += DragRange_MouseMove;
        }

        #region [  Drag 이동  ]

        // Drag Movement에서 쓰일 변수들
        bool captured = false;
        UIElement source;
        double absLeft, relLeft;

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

        public double GetSliderLeft()
        {
            return positioner.Margin.Left;
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

            if (TimeLine != null)
            {
                if (frame == 0 || frame != TimeLine.Position)
                {
                    SetPositionerToPosition();
                }
                if (frame != TimeLine.Position)
                {
                    TimeLine.Position = frame;
                    TimeLine.OnFrameMouseChanged(this, new EventArgs());
                }
            }
        }

        #endregion

        public void SetTrackItem(TrackItem trackItem)
        {
            SelectedTrackItem = trackItem;
            itemName.Text = trackItem?.Text;
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

        protected override void OnRender(DrawingContext dc)
        {
            dc.DrawRectangle(Brushes.Transparent, null, new Rect(0, 0, this.ActualWidth, this.ActualHeight));
            DrawHelperLine(dc);
            base.OnRender(dc);
        }

        public void DrawHelperLine(DrawingContext dc)
        {
            if (TimeLine == null)
                return;

            int startPoint = 184;

            positioner.SetLeftMargin(TimeLine.Position * _realSize - Offset);

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
                    dc.DrawText(new FormattedText(MediaTools.GetTimeText((i + value) * Weight, TimeLine.FrameRate), CultureInfo.GetCultureInfo("en-us"),
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
