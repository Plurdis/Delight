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
using Delight.Commands;
using Delight.Common;
using Delight.Components;
using Delight.Components.Common;
using Delight.Components.Medias;
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

            this.CommandBindings.Add(new CommandBinding(TrackItemCommands.DeleteCommand, DeleteCommandExecuted));
        }

        private void DeleteCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show(sender.ToString());
        }

        public Track(TimeLine parent, TrackType trackType, FrameRate frameRate) : this()
        {
            _parent = parent;
            TrackType = trackType;
            FrameRate = frameRate;
        }

        public event EventHandler ItemsMaxWidthChanged;

        public event EventHandler<ItemEventArgs> ItemAdded;
        public event EventHandler<ItemEventArgs> ItemRemoving;
        public event EventHandler ItemRemoved;

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

        TrackType TrackType
        {
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
                        TrackTypeText = "시각 효과";
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
        FrameRate FrameRate { get; set; } = FrameRate._60FPS;

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
                ItemsMaxWidthChanged?.Invoke(this, new EventArgs());
                ItemAdded?.Invoke(this, new ItemEventArgs(trackItem));
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

            trackItem = null;
        }

        private void ItemCanvas_DragOver(object sender, DragEventArgs e)
        {
            var comp = e.Data.GetData(e.Data.GetFormats()[0]) as StageComponent;

            if (comp == null)
                return;

            if (comp.TrackType != this.TrackType)
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
                return;
            }

            if (trackItem != null)
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
        }

        private void ItemCanvas_DragEnter(object sender, DragEventArgs e)
        {
            var comp = e.Data.GetData(e.Data.GetFormats()[0]) as StageComponent;

            if (comp == null)
                return;

            if (comp.TrackType != this.TrackType)
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
                return;
            }

            if (trackItem == null)
            {
                var frame = MediaTools.TimeSpanToFrame(comp.Time, FrameRate);
                var media = comp as Media;


                trackItem = new TrackItem()
                {
                    FrameWidth = frame,//frame,
                    MaxFrame = frame,
                    IsHitTestVisible = false,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Text = comp.Identifier,
                    Thumbnail = comp.Thumbnail,
                    TrackType = comp.TrackType,
                    OriginalPath = media == null ? "" : media.OriginalPath,
                };

                trackItem.LeftSide_MouseLeftButtonDown += TrackItem_LeftSide_MouseLeftButtonDown;
                trackItem.RightSide_MouseLeftButtonDown += TrackItem_RightSide_MouseLeftButtonDown;
                trackItem.MovingSide_MouseLeftButtonDown += TrackItem_MovingSide_MouseLeftButtonDown;
                trackItem.MouseMove += TrackItem_MouseMove;
                trackItem.MouseLeftButtonUp += TrackItem_MouseLeftButtonUp;

                trackItem.MouseRightButtonClick += TrackItem_MouseRightButtonClick;

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

        private void TrackItem_MouseRightButtonClick(object sender, MouseButtonEventArgs e)
        {
            ContextMenu cm = FindResource("TrackItemContextMenu") as ContextMenu;

            cm.PlacementTarget = ((UIElement)sender);
            cm.IsOpen = true;
        }

        private void TrackItem_MouseMove(object sender, MouseEventArgs e)
        {
            if (captured)
            {
                TrackItem trackItem = element as TrackItem;

                double left = 0, x;
                int diff;

                switch (dragSide)
                {
                    case DragSide.LeftSide:

                        x = Mouse.GetPosition(itemGrid).X;
                        x_canvas += x - x_item;

                        left = x_canvas + Offset;

                        bool skipMagnet = false;

                        double maxLeftMargin = (trackItem.Offset - trackItem.ForwardOffset) * _realSize;
                        Console.WriteLine(maxLeftMargin);
                        if (left < maxLeftMargin)
                        {
                            left = maxLeftMargin;
                            skipMagnet = true;
                        }
                        else if (left < 0)
                        {
                            left = 0;
                        }
                        else if ((x_canvas + trackItem.Margin.Right) >= itemGrid.ActualWidth)
                        {
                            left = itemGrid.ActualWidth - trackItem.Margin.Right - 1;
                        }

                        int leftOffset = (int)((left) / _realSize);

                        // ==================== 마그넷
                        if (!skipMagnet)
                        {
                            int recoSize = 6;

                            if (!((left - recoSize) <= maxLeftMargin)) // 만약 마그넷이 되더라도 최대 길이를 넘지 않는다면 계산
                            {
                                var itemsLeft = Items.Except(new TrackItem[] { trackItem }).Where(i => i.Offset + i.FrameWidth < leftOffset);
                                var itemsRight = Items.Except(new TrackItem[] { trackItem }).Where(i => i.Offset + i.FrameWidth > leftOffset);

                                int lSideMax = itemsLeft.Count() != 0 ? itemsLeft.Max(i => i.Offset + i.FrameWidth) : int.MinValue;
                                int rSideMin = itemsRight.Count() != 0 ? itemsRight.Min(i => i.Offset + i.FrameWidth) : int.MaxValue;

                                double leftBetweenWidth = (leftOffset - lSideMax) * _realSize;
                                bool leftMagnetAllowed = leftBetweenWidth < 6 && leftBetweenWidth >= 0;

                                bool rightMagnetAllowed = ((leftOffset) - rSideMin) * _realSize > -recoSize;

                                if (leftMagnetAllowed)
                                {
                                    leftOffset = lSideMax;
                                }
                                else if (rightMagnetAllowed)
                                {
                                    leftOffset = rSideMin;
                                }
                            }
                        }
                        // ===========================

                        diff = leftOffset - trackItem.Offset;

                        trackItem.ForwardOffset += diff;

                        trackItem.Offset = leftOffset;
                        trackItem.FrameWidth = (int)(fWidth + (fLeft - leftOffset));

                        trackItem.SetLeftMargin((leftOffset * _realSize) - Offset);

                        x_item = x;

                        break;
                    case DragSide.RightSide:
                        x = Mouse.GetPosition(itemGrid).X;

                        // right는 반대 방향으로 감
                        double rightRaw = itemGrid.ActualWidth - (Math.Ceiling(x / _realSize) * _realSize);

                        // ==================================================================
                        // TODO: 특정한 경우 살짝 길이 계산에 오류가 있는거 같음 체크 해보기
                        // ==================================================================

                        double maxMarginRight = itemGrid.ActualWidth - (((trackItem.Offset - trackItem.ForwardOffset + trackItem.MaxFrame) * _realSize) - Offset);

                        if (rightRaw < maxMarginRight) // ForwardOffset 계산해서 전체 길이 이상 늘어나지 못하도록
                            rightRaw = maxMarginRight;
                        if (rightRaw > (itemGrid.ActualWidth - trackItem.Margin.Left)) // 전체 길이보다 작아질 경우
                            rightRaw = (itemGrid.ActualWidth - trackItem.Margin.Left) - _realSize;

                        int width = (int)((itemGrid.ActualWidth - trackItem.Margin.Left - rightRaw) / _realSize);

                        // 마그넷 연구 중 (실패)
                        //Console.WriteLine(width);

                        //{
                        //    var itemsLeft = Items.Except(new TrackItem[] { trackItem }).Where(i => i.Offset < width + trackItem.Offset);
                        //    var itemsRight = Items.Except(new TrackItem[] { trackItem }).Where(i => i.Offset > width + trackItem.Offset);

                        //    int leftMax = itemsLeft.Count() != 0 ? itemsLeft.Max(i => i.Offset) : int.MinValue;
                        //    int rightMin = itemsRight.Count() != 0 ? itemsRight.Min(i => i.Offset) : int.MaxValue;

                        //    double rightBetweenWidth = (trackItem.Offset + width - rightMin) * _realSize;

                        //    if (rightBetweenWidth < 6)
                        //    {
                        //        width = rightMin;
                        //        rightRaw = ActualWidth - (width / _realSize);
                        //    }

                        //}


                        //// ==================== 마그넷
                        //{
                        //    var itemsLeft = Items.Except(new TrackItem[] { trackItem }).Where(i => i.Offset + i.FrameWidth < leftOffset);
                        //    var itemsRight = Items.Except(new TrackItem[] { trackItem }).Where(i => i.Offset + i.FrameWidth > leftOffset);

                        //    int leftMax = itemsLeft.Count() != 0 ? itemsLeft.Max(i => i.Offset + i.FrameWidth) : int.MinValue;
                        //    int rightMin = itemsRight.Count() != 0 ? itemsRight.Min(i => i.Offset + i.FrameWidth) : int.MaxValue;

                        //    double leftBetweenWidth = (leftOffset - leftMax) * _realSize;
                        //    bool leftMagnetAllowed = leftBetweenWidth < 6 && leftBetweenWidth >= 0;

                        //    bool rightMagnetAllowed = ((leftOffset) - rightMin) * _realSize > -6;

                        //    if (leftMagnetAllowed)
                        //    {
                        //        leftOffset = leftMax;
                        //    }
                        //    else if (rightMagnetAllowed)
                        //    {
                        //        leftOffset = rightMin;
                        //    }
                        //}

                        //// ===========================


                        diff = width - trackItem.FrameWidth;
                        trackItem.BackwardOffset -= diff;


                        trackItem.SetRightMargin(rightRaw);
                        trackItem.FrameWidth = width;
                        break;
                    case DragSide.MovingSide:
                        x = e.GetPosition(itemGrid).X;
                        x_item += x - x_canvas;
                        x_canvas = x;

                        left = x_item + Offset;

                        if (left < 0)
                        {
                            x_canvas -= left;
                            x_item = -Offset;
                            left = 0;
                        }

                        trackItem.SetLeftMargin(((int)(x_item / _realSize)) * _realSize);
                        trackItem.SetRightMargin(itemGrid.ActualWidth - trackItem.Margin.Left - trackItem.ActualWidth);

                        trackItem.Offset = (int)(left / _realSize);
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
            foreach (TrackItem item in itemGrid.Children)
            {
                item.SetLeftMargin(item.Offset * _realSize - _parent.Offset);
                item.SetRightMargin(itemGrid.ActualWidth - item.Margin.Left - (item.FrameWidth * _realSize)); // + _parent.Offset
            }
        }

        #endregion
    }
}
