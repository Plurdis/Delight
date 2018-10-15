using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Delight.Components.Common;
using Delight.Core.Extension;

namespace Delight.Controls
{
    /// <summary>
    /// TrackItemEditor.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TrackItemEditor : UserControl
    {
        public TrackItemEditor()
        {
            InitializeComponent();

            slOpacity.PropertyChanged += SlOpacity_PropertyChanged;

            this.Loaded += (s, e) => SetTrackItem(null);

            //slSpeed.ValueChanged += SlSpeed_ValueChanged;
            slSize.PropertyChanged += SlSize_PropertyChanged;
            slVolume.PropertyChanged += SlVolume_PropertyChanged;
            slX.PropertyChanged += SlX_PropertyChanged;
            slY.PropertyChanged += SlY_PropertyChanged;
            slChromaUsage.PropertyChanged += SlChromaUsage_PropertyChanged;
            pickChromaColor.PropertyChanged += PickChromaColor_PropertyChanged;
            cbChromaKey.PropertyChanged += CbChromaKey_PropertyChanged;
        }

        private void CbChromaKey_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (item != null)
                item.ItemProperty.ChromaKeyEnabled = cbChromaKey.IsChecked;
        }

        private void PickChromaColor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (item != null)
                item.ItemProperty.ChromaKeyColor = pickChromaColor.SelectedColor;
        }

        private void SlChromaUsage_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (item != null)
                item.ItemProperty.ChromaKeyUsage = slChromaUsage.Value;
        }

        private void SlX_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (item != null)
                item.ItemProperty.PositionX = slX.Value;
        }

        private void SlY_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (item != null)
                item.ItemProperty.PositionY = slY.Value;
        }

        private void SlVolume_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (item != null)
                item.ItemProperty.Volume = slVolume.Value;
        }

        private void SlSize_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (item != null)
                item.ItemProperty.Size = slSize.Value;
        }

        private void SlOpacity_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (item != null)
                item.ItemProperty.Opacity = slOpacity.Value;
        }

        private void SlSpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //double speed = Math.Round(slSpeed.Value, 2);
        }

        TrackItem item;

        public void SetTrackItem(TrackItem trackItem)
        {
            string name = string.Empty, type = string.Empty;
            ImageSource image = null;

            item = null;
            if (trackItem != null)
            {
                name = trackItem.Text;
                type = trackItem.TrackType.GetEnumAttribute<DescriptionAttribute>().Description;
                image = trackItem.Thumbnail;

                item = trackItem;

                slOpacity.Value = trackItem.ItemProperty.Opacity;
                slSize.Value = trackItem.ItemProperty.Size;
                slVolume.Value = trackItem.ItemProperty.Volume;
                cbChromaKey.IsChecked = trackItem.ItemProperty.ChromaKeyEnabled;
                pickChromaColor.SelectedColor = trackItem.ItemProperty.ChromaKeyColor;
                slChromaUsage.Value = trackItem.ItemProperty.ChromaKeyUsage;
                // ERROR: 오류가 난다면 이쪽을!
                runLength.Text = MediaTools.GetTimeText(trackItem.FrameWidth, Core.Common.FrameRate._60FPS);
            }

            tbName.Text = name;
            rType.Text = type;
            img.Source = image;

            this.Visibility = (trackItem == null) ? Visibility.Hidden : Visibility.Visible;
        }
    }
}
