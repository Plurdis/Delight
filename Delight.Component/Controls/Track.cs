using Delight.Component.Common;
using Delight.Component.Extensions;
using Delight.Component.ItemProperties;
using Delight.Core.Common;
using Delight.Core.Extensions;
using Delight.Core.Stage;
using Delight.Core.Stage.Components;
using Delight.Core.Stage.Components.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

#pragma warning disable CS0067

namespace Delight.Component.Controls
{
    [TemplatePart(Name = "trackMenuSide", Type = typeof(Grid))]
    [TemplatePart(Name = "itemGrid", Type = typeof(Grid))]
    public class Track : Control
    {
        public Track()
        {
            this.Style = FindResource("TrackStyle") as Style;

            //helper = new MagnetHelper(this, 10);

            this.ApplyTemplate();
        }

        public Track(TimeLine parent, SourceType sourceType) : this()
        {
            _parent = parent;
            SourceType = sourceType;
        }

        #region [  Events  ]

        public event EventHandler ItemsMaxWidthChanged;

        public event EventHandler<ItemEventArgs> ItemAdded;
        public event EventHandler<ItemEventArgs> ItemRemoving;
        public event EventHandler ItemRemoved;

        #endregion

        #region [  Dependency Properties  ]

        public static DependencyProperty TrackNumberProperty = DependencyProperty.Register(nameof(TrackNumber), typeof(int), typeof(Track));

        public int TrackNumber
        {
            get => (int)GetValue(TrackNumberProperty);
            set => SetValue(TrackNumberProperty, value);
        }

        public static DependencyProperty SourceTypeTextProperty = DependencyProperty.Register(nameof(SourceTypeText), typeof(string), typeof(Track));

        public string SourceTypeText
        {
            get => GetValue(SourceTypeTextProperty) as string;
            set => SetValue(SourceTypeTextProperty, value);
        }

        #endregion

        #region [  Local Variables  ]

        FrameworkElement element;
        TimeLine _parent;
        //MagnetHelper helper;

        bool captured;
        double x_item, x_canvas;
        double fLeft;
        int fWidth;
        DragSide dragSide = DragSide.Unknown;
        SourceType _sourceType = SourceType.Unknown;
        static TrackItem trackItem;
        Grid itemGrid;
        Grid trackMenuSide;

        #endregion

        #region [  Properties  ]

        public double ItemSize => 0.25;

        private double _realSize => ItemSize * Ratio;

        private double Ratio => _parent.Ratio;

        public double ItemsMaxWidth => IsEmpty ? 0 : Items.Select(i => (i.Offset + i.FrameWidth) * _realSize).Max();

        public int ItemsMaxFrame => IsEmpty ? 0 : Items.Select(i => (i.Offset + i.FrameWidth)).Max();

        public double Offset => _parent.Offset;

        public bool IsEmpty => itemGrid.Children.Count == 0;

        public IEnumerable<TrackItem> Items => itemGrid.Children.Cast<TrackItem>();

        public FrameRate FrameRate => _parent.FrameRate;

        public SourceType SourceType
        {
            set
            {
                SourceTypeText = value.GetAttribute<DescriptionAttribute>()?.Description;
                _sourceType = value;
            }
            get => _sourceType;
        }

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            trackMenuSide = GetTemplateChild("trackMenuSide") as Grid;
            trackMenuSide.MouseRightButtonDown += TrackMenuSide_MouseRightButtonDown;

            itemGrid = GetTemplateChild("itemGrid") as Grid;
            itemGrid.DragEnter += ItemCanvas_DragEnter;
            itemGrid.DragOver += ItemCanvas_DragOver;
            itemGrid.DragLeave += ItemCanvas_DragLeave;
            itemGrid.Drop += ItemCanvas_Drop;
        }

        private void TrackMenuSide_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        #region [  TrackItem Drag & Drop  ] 

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
                itemGrid.Children.Remove(trackItem);

            trackItem = null;
        }

        private void ItemCanvas_DragOver(object sender, DragEventArgs e)
        {
            var comp = e.Data.GetData(e.Data.GetFormats()[0]) as StageComponent;

            if (comp == null)
                return;

            if (comp.SourceType != this.SourceType)
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

                if (trackItem.Visibility == Visibility.Hidden)
                {
                    trackItem.Visibility = Visibility.Visible;
                }
            }
        }

        public TrackItem BuildItem(StageComponent component)
        {
            var frame = MediaTools.TimeSpanToFrame(component.Time, FrameRate);
            var media = component as BaseMedia;

            var trackItem = new TrackItem()
            {
                FrameWidth = frame,//frame,
                MaxFrame = frame,
                IsHitTestVisible = false,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Text = component.Identifier,
                Thumbnail = new BitmapImage(component.Thumbnail),
                SourceType = component.SourceType,
                OriginalPath = media == null ? string.Empty : media.Path,
                MaxSizeFixed = !component.IsDynamicLength,
                Visibility = Visibility.Hidden,
                Tag = component,
            };

            switch (component.SourceType)
            {
                case SourceType.Video:
                    trackItem.Property = new VideoItemProperty();
                    break;
                case SourceType.Light:
                    var lComp = component as LightComponent;
                    trackItem.Property = lComp.SetterBoard.GetSetterBaseProperty();
                    break;
            }

            trackItem.LeftSide_MouseLeftButtonDown += TrackItem_LeftSide_MouseLeftButtonDown;
            trackItem.RightSide_MouseLeftButtonDown += TrackItem_RightSide_MouseLeftButtonDown;
            trackItem.MovingSide_MouseLeftButtonDown += TrackItem_MovingSide_MouseLeftButtonDown;
            trackItem.MouseMove += TrackItem_MouseMove;
            trackItem.MouseLeftButtonUp += TrackItem_MouseLeftButtonUp;
            trackItem.MouseRightButtonClick += TrackItem_MouseRightButtonClick;

            return trackItem;
        }

        private void ItemCanvas_DragEnter(object sender, DragEventArgs e)
        {
            var comp = e.Data.GetData(e.Data.GetFormats()[0]) as StageComponent;

            if (comp == null)
                return;

            if (comp.SourceType != this.SourceType)
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
                return;
            }

            if (trackItem == null)
            {
                trackItem = BuildItem(comp);
            }
            if (!itemGrid.Children.Contains(trackItem))
            {
                itemGrid.Children.Add(trackItem);

                x_canvas = e.GetPosition(itemGrid).X;

                double cl = trackItem.Margin.Left;
                if (double.IsNaN(cl))
                    cl = 0;

                x_item = cl + Mouse.GetPosition(trackItem).X;

                trackItem.SetLeftMargin(x_item);
            }
        }

        public void AddItem(TrackItem item)
        {
            itemGrid.Children.Add(item);

            //item.SetLeftMargin(item.Offset * _realSize);
            //item.SetRightMargin(itemGrid.ActualWidth - (item.FrameWidth * _realSize) + item.Margin.Left);
            item.Visibility = Visibility.Visible;
            item.IsHitTestVisible = true;
        }

        #endregion

        #region [  TrackItem Events  ]

        private void TrackItem_MouseRightButtonClick(object sender, MouseButtonEventArgs e)
        {
            RemoveItem(sender as TrackItem);

            //ContextMenu cm = FindResource("TrackItemContextMenu") as ContextMenu;

            //cm.PlacementTarget = ((UIElement)sender);
            //cm.IsOpen = true;
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
                    case DragSide.Left:

                        x = Mouse.GetPosition(itemGrid).X;
                        x_canvas += x - x_item;

                        left = x_canvas + Offset;

                        bool skipMagnet = false;
                        // TODO: 길이 제한이 없을 경우에도 마그넷 기능 넣도록 리팩토링
                        double maxLeftMargin = (trackItem.Offset - trackItem.ForwardOffset) * _realSize;
                        if (trackItem.MaxSizeFixed)
                        {
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

                        }

                        int leftOffset = (int)((left) / _realSize);

                        // ==================== 마그넷
                        if (!skipMagnet)
                        {
                            //int recoSize = 6;

                            //if (!((left - recoSize) <= maxLeftMargin)) // 만약 마그넷이 되더라도 최대 길이를 넘지 않는다면 계산
                            //{
                            //    var itemsLeft = Items.Except(new TrackItem[] { trackItem }).Where(i => i.Offset + i.FrameWidth < leftOffset);
                            //    var itemsRight = Items.Except(new TrackItem[] { trackItem }).Where(i => i.Offset + i.FrameWidth > leftOffset);

                            //    int lSideMax = itemsLeft.Count() != 0 ? itemsLeft.Max(i => i.Offset + i.FrameWidth) : int.MinValue;
                            //    int rSideMin = itemsRight.Count() != 0 ? itemsRight.Min(i => i.Offset + i.FrameWidth) : int.MaxValue;

                            //    double leftBetweenWidth = (leftOffset - lSideMax) * _realSize;
                            //    bool leftMagnetAllowed = leftBetweenWidth < 6 && leftBetweenWidth >= 0;

                            //    bool rightMagnetAllowed = ((leftOffset) - rSideMin) * _realSize > -recoSize;

                            //    if (leftMagnetAllowed)
                            //    {
                            //        leftOffset = lSideMax;
                            //    }
                            //    else if (rightMagnetAllowed)
                            //    {
                            //        leftOffset = rSideMin;
                            //    }
                            //}




                            //leftOffset = 

                        }
                        // ===========================

                        diff = leftOffset - trackItem.Offset;

                        trackItem.ForwardOffset += diff;

                        trackItem.Offset = leftOffset;
                        trackItem.FrameWidth = (int)(fWidth + (fLeft - leftOffset));

                        trackItem.SetLeftMargin((leftOffset * _realSize) - Offset);

                        x_item = x;

                        break;
                    case DragSide.Right:
                        x = Mouse.GetPosition(itemGrid).X;

                        // right는 반대 방향으로 감
                        double rightRaw = itemGrid.ActualWidth - (Math.Ceiling(x / _realSize) * _realSize);

                        // ==================================================================
                        // TODO: 특정한 경우 살짝 길이 계산에 오류가 있는거 같음 체크 해보기
                        // ==================================================================

                        double maxMarginRight = itemGrid.ActualWidth - (((trackItem.Offset - trackItem.ForwardOffset + trackItem.MaxFrame) * _realSize) - Offset);

                        if (rightRaw < maxMarginRight && trackItem.MaxSizeFixed) // ForwardOffset 계산해서 전체 길이 이상 늘어나지 못하도록
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
                    case DragSide.Move:
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
                dragSide = DragSide.Move;
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
                dragSide = DragSide.Left;
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

                dragSide = DragSide.Right;
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

        public void RemoveItem(TrackItem item)
        {
            ItemRemoving?.Invoke(this, new ItemEventArgs(item));
            itemGrid.Children.Remove(item);
            ItemRemoved?.Invoke(this, new EventArgs());
        }
    }
}
