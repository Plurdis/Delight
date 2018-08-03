using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Delight.Common;
using Delight.Components;
using Delight.Components.Common;
using Delight.Core.Common;
using Delight.Extensions;
using Delight.TimeLineComponents;

using f = System.Windows.Forms;

namespace Delight.Controls
{
    [TemplatePart(Name = "itemGrid", Type = typeof(Grid))]
    public class Track : Control
    {
        public Track()
        {
            this.Style = FindResource("TrackStyle") as Style;
        }
        public Track(TimeLine parent, TrackType trackType, FrameRate frameRate) : this()
        {
            _parent = parent;
            TrackType = trackType;
            FrameRate = frameRate;
        }

        public event EventHandler ItemsMaxWidthChanged;

        static TrackItem trackItem;
        Grid itemGrid;
        TimeLine _parent;

        public static DependencyProperty TrackTypeTextProperty = DependencyProperty.Register(nameof(TrackTypeText), typeof(string), typeof(Track));

        public string TrackTypeText
        {
            get => GetValue(TrackTypeTextProperty) as string;
            set => SetValue(TrackTypeTextProperty, value);
        }

        TrackType _trackType = TrackType.Unknown;

        TrackType TrackType {
            set
            {
                switch (value)
                {
                    case TrackType.Image:
                        TrackTypeText = "이미지";
                        break;
                    case TrackType.Video:
                        TrackTypeText = "비디오";
                        break;
                    case TrackType.Unity:
                        TrackTypeText = "유니티";
                        break;
                    case TrackType.Sound:
                        TrackTypeText = "사운드";
                        break;
                    case TrackType.Unknown:
                        break;
                    default:
                        break;
                }

                _trackType = value;
            }
            get
            {
                return _trackType;
            }
        } 
        FrameRate FrameRate { get; set; } = FrameRate._24FPS;

        FrameworkElement element;
        DragSide dragSide = DragSide.Unknown;
        bool captured;
        double x_item, x_canvas;
        int fWidth;
        double fLeft;

        public double ItemSize = 0.25;
        private double _realSize => ItemSize * Ratio;

        private double Ratio => _parent.Ratio;

        public double ItemsMaxWidth => IsEmpty ? 0 : Items.Select(i => (i.Offset + i.FrameWidth) * _realSize).Max();

        public int ItemsMaxFrame => IsEmpty ? 0 : Items.Select(i => (i.Offset + i.FrameWidth)).Max();

        public double Offset => _parent.Offset;

        public bool IsEmpty => itemGrid.Children.Count == 0;

        public IEnumerable<TrackItem> Items => itemGrid.Children.Cast<TrackItem>();

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            itemGrid = GetTemplateChild("itemGrid") as Grid;
            itemGrid.DragEnter += ItemCanvas_DragEnter;
            itemGrid.DragOver += ItemCanvas_DragOver;
            itemGrid.DragLeave += ItemCanvas_DragLeave;
            itemGrid.Drop += ItemCanvas_Drop;
        }

        #region [  Item Drag & Drop  ] 

        private void ItemCanvas_Drop(object sender, DragEventArgs e)
        {
            if (trackItem != null)
            {
                ItemsMaxWidthChanged?.Invoke(trackItem, new EventArgs());

                trackItem.IsHitTestVisible = true;
                trackItem = null;
            }
        }

        private void ItemCanvas_DragLeave(object sender, DragEventArgs e)
        {
            if (trackItem != null)
            {
                itemGrid.Children.Remove(trackItem);
            }
        }

        private void ItemCanvas_DragOver(object sender, DragEventArgs e)
        {
            double x = e.GetPosition(itemGrid).X;
            x_canvas += x - x_item;

            double left = (int)(x_item / _realSize) * _realSize;
            int leftRaw = (int)(x_item / _realSize);

            trackItem.SetLeftMargin(left);



            double right = itemGrid.ActualWidth - (trackItem.FrameWidth * _realSize) - x_item;
            right = (int)(right / _realSize) * _realSize;

            double rightRaw = (int)(right / _realSize);

            trackItem.SetRightMargin(right);
            trackItem.Offset = leftRaw;

            x_item = x;

            
        }

        private void ItemCanvas_DragEnter(object sender, DragEventArgs e)
        {
            if (trackItem == null)
            {
                var comp = e.Data.GetData(e.Data.GetFormats()[0]) as StageComponent;
                var frame = MediaTools.TimeSpanToFrame(comp.Time, FrameRate);

                trackItem = new TrackItem()
                {
                    FrameWidth = frame,//frame,
                    MaxFrame = frame,
                    IsHitTestVisible = false,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Text = comp.Identifier,
                    Thumbnail = comp.Thumbnail,
                    TrackType = comp.TrackType,
                };

                trackItem.LeftSide_MouseLeftButtonDown += TrackItem_LeftSide_MouseLeftButtonDown;
                trackItem.RightSide_MouseLeftButtonDown += TrackItem_RightSide_MouseLeftButtonDown;
                trackItem.MovingSide_MouseLeftButtonDown += TrackItem_MovingSide_MouseLeftButtonDown;
                trackItem.MouseMove += TrackItem_MouseMove;
                trackItem.MouseLeftButtonUp += TrackItem_MouseLeftButtonUp;

            }
            if (!itemGrid.Children.Contains(trackItem))
            {
                itemGrid.Children.Add(trackItem);
                x_canvas = e.GetPosition(itemGrid).X;

                double cl = trackItem.Margin.Left;
                if (Double.IsNaN(cl))
                    cl = 0;

                x_item = cl + Mouse.GetPosition(trackItem).X;

                trackItem.SetLeftMargin(x_item);
                
            }
        }

        private void TrackItem_MouseMove(object sender, MouseEventArgs e)
        {
            if (captured)
            {
                switch (dragSide)
                {
                    case DragSide.LeftSide:
                        if (element is TrackItem ti_l)
                        {
                            double x = Mouse.GetPosition(itemGrid).X;
                            x_canvas += x - x_item;

                            double left = x_canvas + Offset;


                            if (left < (ti_l.Offset - ti_l.ForwardOffset) * _realSize)
                            {
                                left = (ti_l.Offset - ti_l.ForwardOffset) * _realSize;
                            }
                            else if (left < 0)
                            {
                                left = 0;
                            }
                            else if ((x_canvas + ti_l.Margin.Right) >= itemGrid.ActualWidth)
                            {
                                left = itemGrid.ActualWidth - ti_l.Margin.Right - 1;
                            }

                            int leftOffset = (int)((left) / _realSize);

                            int diff = leftOffset - ti_l.Offset;

                            ti_l.ForwardOffset += diff;
                            
                            ti_l.Offset = leftOffset;
                            ti_l.FrameWidth = (int)(fWidth + (fLeft - leftOffset));

                            ti_l.SetLeftMargin((leftOffset * _realSize) - Offset);
                            
                            x_item = x;
                        }
                        
                        break;
                    case DragSide.RightSide:
                        if (element is TrackItem tI_r)
                        {
                            
                            double x = Mouse.GetPosition(itemGrid).X;

                            // right는 반대 방향으로 감
                            double rightRaw = itemGrid.ActualWidth - (Math.Ceiling(x / _realSize) * _realSize);

                            // ==================================================================
                            // TODO: 특정한 경우 살짝 길이 계산에 오류가 있는거 같음 체크 해보기
                            // ==================================================================


                            if (rightRaw < itemGrid.ActualWidth - (((tI_r.Offset - tI_r.ForwardOffset + tI_r.MaxFrame) * _realSize) - Offset)) // ForwardOffset 계산해서 전체 길이 이상 늘어나지 못하도록
                                rightRaw = itemGrid.ActualWidth - (((tI_r.Offset - tI_r.ForwardOffset + tI_r.MaxFrame) * _realSize) - Offset);
                            if (rightRaw > (itemGrid.ActualWidth - tI_r.Margin.Left)) // 전체 길이보다 작아질 경우
                                rightRaw = (itemGrid.ActualWidth - tI_r.Margin.Left) - _realSize;

                            tI_r.SetRightMargin(rightRaw);


                            int width = (int)((itemGrid.ActualWidth - tI_r.Margin.Left - tI_r.Margin.Right) / _realSize);

                            int diff = width - tI_r.FrameWidth;
                            tI_r.BackwardOffset -= diff;

                            

                            tI_r.FrameWidth = width;
                        }

                        break;
                    case DragSide.MovingSide:
                        if (element is TrackItem trackItem_m)
                        {
                            double x = e.GetPosition(itemGrid).X;
                            x_item += x - x_canvas;
                            x_canvas = x;
                            
                            double left = x_item + Offset;
                            
                            if (left < 0)
                            {
                                x_canvas -= left;
                                x_item = -Offset;
                                left = 0;
                            }

                            trackItem_m.SetLeftMargin(((int)(x_item / _realSize)) * _realSize);
                            trackItem_m.SetRightMargin(itemGrid.ActualWidth - trackItem_m.Margin.Left - trackItem_m.ActualWidth);

                            trackItem_m.Offset = (int)(left / _realSize);
                            
                        }

                        break;
                    case DragSide.Unknown:
                        break;
                    default:
                        break;
                }
            }
        }

        private void TrackItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (captured)
                ItemsMaxWidthChanged?.Invoke(element, new EventArgs());
            captured = false;
            dragSide = DragSide.Unknown;
            Mouse.Capture(null);
        }

        #endregion

        #region [  TrackItem Side MouseLeftButtonDown Events  ]

        private void TrackItem_MovingSide_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (((FrameworkElement)sender).TemplatedParent is TrackItem trackItem)
            {
                element = trackItem;

                fWidth = trackItem.FrameWidth;
                fLeft = trackItem.Offset;

                Mouse.Capture(trackItem);
                x_item = trackItem.Margin.Left;
                x_canvas = e.GetPosition(itemGrid).X;
                dragSide = DragSide.MovingSide;
                captured = true;
            }
        }

        private void TrackItem_LeftSide_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (((FrameworkElement)sender).TemplatedParent is TrackItem trackItem)
            {
                element = trackItem;

                fWidth = trackItem.FrameWidth;
                fLeft = trackItem.Offset;

                Mouse.Capture(trackItem);
                x_canvas = Mouse.GetPosition(itemGrid).X;
                x_item = trackItem.Margin.Left + Mouse.GetPosition(trackItem).X;

                trackItem.SetLeftMargin(x_item);
                dragSide = DragSide.LeftSide;
                captured = true;
            }
        }

        private void TrackItem_RightSide_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (((FrameworkElement)sender).TemplatedParent is TrackItem trackItem)
            {
                element = trackItem;
                Mouse.Capture(trackItem);
                double x = Mouse.GetPosition(itemGrid).X;

                double rightRaw = itemGrid.ActualWidth - (Math.Ceiling(x / _realSize) * _realSize);

                if (rightRaw > (itemGrid.ActualWidth - trackItem.Margin.Left))
                    rightRaw = (itemGrid.ActualWidth - trackItem.Margin.Left) - _realSize;

                trackItem.SetRightMargin(rightRaw);
                trackItem.FrameWidth = (int)((itemGrid.ActualWidth - trackItem.Margin.Left - trackItem.Margin.Right) / _realSize);

                x_item = x;

                dragSide = DragSide.RightSide;
                captured = true;
            }
        }

        #endregion

        #region [  Relocate Items  ]

        public void RelocationTrackItems()
        {
            foreach(TrackItem item in itemGrid.Children)
            {
                item.SetLeftMargin(item.Offset * _realSize - _parent.Offset);
                item.SetRightMargin(itemGrid.ActualWidth - item.Margin.Left - (item.FrameWidth * _realSize)); // + _parent.Offset
            }
        }

    #endregion
    }
}
