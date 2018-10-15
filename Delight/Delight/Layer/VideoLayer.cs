using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Effects;
using Delight.Controls;
using Delight.Effects;

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

            //var mw = Application.Current.MainWindow as MainWindow;

            Player1 = GetTemplateChild("player1") as MediaElementPro;
            Player2 = GetTemplateChild("player2") as MediaElementPro;

            //DependencyProperty[] properties = new DependencyProperty[] { ChromaKeyEffect.HueMinProperty, ChromaKeyEffect.HueMaxProperty,
            //                                                             ChromaKeyEffect.SaturationMinProperty, ChromaKeyEffect.SaturationMaxProperty,
            //                                                             ChromaKeyEffect.LuminanceMinProperty, ChromaKeyEffect.LuminanceMaxProperty,
            //                                                             ChromaKeyEffect.SmoothProperty };

            //Slider[] sliders = new Slider[] { mw.sliderHueMin, mw.sliderHueMax, mw.sliderSatMin, mw.sliderSatMax, mw.sliderLightMin, mw.sliderLightMax, mw.sliderSmooth };

            //for (int index = 0; index < 7; index++)
            //{
            //    BindingOperations.SetBinding(Player1.Effect, properties[index], new Binding()
            //    {
            //        Source = sliders[index],
            //        Path = new PropertyPath("Value"),
            //    });

            //    BindingOperations.SetBinding(Player2.Effect, properties[index], new Binding()
            //    {
            //        Source = sliders[index],
            //        Path = new PropertyPath("Value"),
            //    });
            //}
        }

        public MediaElementPro Player1 { get; private set; }

        public MediaElementPro Player2 { get; private set; }
    }
}
