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


        TrackType TrackType { get; set; } = TrackType.Unknown;
        FrameRate FrameRate { get; set; } = FrameRate._24FPS;

        FrameworkElement element;
        DragSide dragSide = DragSide.Unknown;
        bool captured;
        double x_item, x_canvas;
        int fWidth;
        double fLeft, fRight;

        public double ItemSize = 0.25;
        private double _realSize => ItemSize * Ratio;

        private double Ratio => _parent.Ratio;

        public double ItemsMaxWidth => itemGrid.Children.Count == 0 ? 0 : itemGrid.Children.Cast<TrackItem>().Select(i => (i.Offset + i.FrameWidth) * _realSize).Max();

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
                        if (element is TrackItem trackItem_l)
                        {
                            double x = Mouse.GetPosition(itemGrid).X;

                            x_canvas += x - x_item;

                            double left = x_canvas;

                            if (x_canvas < 0)
                            {
                                left = 0;
                            }
                            else if ((x_canvas + trackItem_l.Margin.Right) >= itemGrid.ActualWidth)
                            {
                                left = itemGrid.ActualWidth - trackItem_l.Margin.Right - 1;
                            }

                            int leftOffset = (int)(left / _realSize);

                            trackItem_l.Offset = leftOffset;
                            trackItem_l.FrameWidth = (int)(fWidth + (fLeft - leftOffset));

                            trackItem_l.SetLeftMargin(leftOffset * _realSize);
                            
                            x_item = x;
                        }
                        
                        break;
                    case DragSide.RightSide:
                        if (element is TrackItem trackItem_r)
                        {
                            double x = Mouse.GetPosition(itemGrid).X;

                            double rightRaw = itemGrid.ActualWidth - (Math.Ceiling(x / _realSize) * _realSize);

                            if (rightRaw > (itemGrid.ActualWidth - trackItem_r.Margin.Left))
                                rightRaw = (itemGrid.ActualWidth - trackItem_r.Margin.Left) - _realSize;

                            trackItem_r.SetRightMargin(rightRaw);
                            trackItem_r.FrameWidth = (int)((itemGrid.ActualWidth - trackItem_r.Margin.Left - trackItem_r.Margin.Right) / _realSize);
                        }

                        break;
                    case DragSide.MovingSide:
                        if (element is TrackItem trackItem_m)
                        {
                            double x = e.GetPosition(itemGrid).X;
                            x_item += x - x_canvas;
                            x_canvas = x;

                            if (x_item < 0)
                            {
                                x_canvas -= x_item;
                                x_item = 0;
                            }

                            trackItem_m.SetLeftMargin(((int)(x_item / _realSize)) * _realSize);
                            trackItem_m.SetRightMargin(itemGrid.ActualWidth - trackItem_m.Margin.Left - trackItem_m.ActualWidth);

                            trackItem_m.Offset = (int)(x_item / _realSize);
                            Console.WriteLine(ItemsMaxWidth);
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
