using Delight.Component.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Delight.Component.Layers
{
    [TemplatePart(Name = "image", Type = typeof(Image))]
    public class ImageLayer : BaseLayer
    {
        public ImageLayer(Track track) : base(track)
        {
            this.Style = FindResource("ImageLayerStyle") as Style;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.Image = GetTemplateChild("image") as Image;
        }


        public Image Image { get; private set; }

        public ImageSource Source
        {
            get => Image.Source;
            set => Image.Source = value;
        }
    }
}
