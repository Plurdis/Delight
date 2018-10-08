using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using Delight.Controls;

namespace Delight.Layer
{
    [TemplatePart(Name = "player1", Type = typeof(MediaElementPro))]
    [TemplatePart(Name = "player2", Type = typeof(MediaElementPro))]
    public class VideoLayer : BaseLayer
    {
        public VideoLayer(Track track) : base(track)
        {
            this.Style = FindResource("VideoLayerStyle") as Style;

            ApplyTemplate();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Player1 = GetTemplateChild("player1") as MediaElementPro;
            Player2 = GetTemplateChild("player2") as MediaElementPro;
        }

        public MediaElementPro Player1 { get; private set; }

        public MediaElementPro Player2 { get; private set; }
    }
}
