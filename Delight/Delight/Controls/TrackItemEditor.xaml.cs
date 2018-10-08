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

            slOpacity.ValueChanged += SlOpacity_ValueChanged;

            this.Loaded += (s, e) => SetTrackItem(null);

            slSpeed.ValueChanged += SlSpeed_ValueChanged;
            slSize.ValueChanged += SlSize_ValueChanged;
        }

        private void SlSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            item.ItemProperty.Size = e.NewValue;
            runSize.Text = ((int)(e.NewValue * 100)).ToString();

            Console.WriteLine(item.ItemProperty.Size);
        }

        private void SlSpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //double speed = Math.Round(slSpeed.Value, 2);
        }

        private void SlOpacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            item.ItemProperty.Opacity = e.NewValue;
            runOpacity.Text = ((int)(e.NewValue * 100)).ToString();
        }

        TrackItem item;

        public void SetTrackItem(TrackItem trackItem)
        {
            string name = string.Empty, type = string.Empty;
            double size = 0, opacity = 0;
            ImageSource image = null;

            item = null;
            if (trackItem != null)
            {
                name = trackItem.Text;
                type = trackItem.TrackType.GetEnumAttribute<DescriptionAttribute>().Description;
                image = trackItem.Thumbnail;
                item = trackItem;
                size = trackItem.ItemProperty.Size;
                opacity = trackItem.ItemProperty.Opacity;
            }

            tbName.Text = name;
            rType.Text = type;
            img.Source = image;
            this.Visibility = (trackItem == null) ? Visibility.Hidden : Visibility.Visible;
        }
    }
}
