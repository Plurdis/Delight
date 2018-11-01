using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Delight.Component.Effects
{
    public class ChromaKeyEffect : ShaderEffect
    {
        public static DependencyProperty BrushProperty =
            RegisterPixelShaderSamplerProperty(nameof(Brush), typeof(ChromaKeyEffect), 0);

        public static DependencyProperty HueMinProperty =
            DependencyProperty.Register(nameof(HueMin),
                typeof(float),
                typeof(ChromaKeyEffect),
                new PropertyMetadata(0F, PixelShaderConstantCallback(0)));

        public static DependencyProperty HueMaxProperty =
            DependencyProperty.Register(nameof(HueMax),
                typeof(float),
                typeof(ChromaKeyEffect),
                new PropertyMetadata(0F, PixelShaderConstantCallback(1)));

        public static DependencyProperty SaturationMinProperty =
            DependencyProperty.Register(nameof(SaturationMin),
                typeof(float),
                typeof(ChromaKeyEffect),
                new PropertyMetadata(0F, PixelShaderConstantCallback(2)));

        public static DependencyProperty SaturationMaxProperty =
            DependencyProperty.Register(nameof(SaturationMax),
                typeof(float),
                typeof(ChromaKeyEffect),
                new PropertyMetadata(1F, PixelShaderConstantCallback(3)));

        public static DependencyProperty LuminanceMinProperty =
            DependencyProperty.Register(nameof(LuminanceMin),
                typeof(float),
                typeof(ChromaKeyEffect),
                new PropertyMetadata(0F, PixelShaderConstantCallback(4)));

        public static DependencyProperty LuminanceMaxProperty =
            DependencyProperty.Register(nameof(LuminanceMax),
                typeof(float),
                typeof(ChromaKeyEffect),
                new PropertyMetadata(1F, PixelShaderConstantCallback(5)));

        public static DependencyProperty SmoothProperty =
            DependencyProperty.Register(nameof(Smooth),
                typeof(float),
                typeof(ChromaKeyEffect),
                new PropertyMetadata(0F, PixelShaderConstantCallback(6)));

        public Brush Brush
        {
            get => GetValue(BrushProperty) as Brush;
            set => SetValue(BrushProperty, value);
        }

        public float HueMin
        {
            get => (float)GetValue(HueMinProperty);
            set => SetValue(HueMinProperty, value);
        }

        public float HueMax
        {
            get => (float)GetValue(HueMaxProperty);
            set => SetValue(HueMaxProperty, value);
        }

        public float SaturationMin
        {
            get => (float)GetValue(SaturationMinProperty);
            set => SetValue(SaturationMinProperty, value);
        }

        public float SaturationMax
        {
            get => (float)GetValue(SaturationMaxProperty);
            set => SetValue(SaturationMaxProperty, value);
        }

        public float LuminanceMin
        {
            get => (float)GetValue(LuminanceMinProperty);
            set => SetValue(LuminanceMinProperty, value);
        }

        public float LuminanceMax
        {
            get => (float)GetValue(LuminanceMaxProperty);
            set => SetValue(LuminanceMaxProperty, value);
        }

        public float Smooth
        {
            get => (float)GetValue(SmoothProperty);
            set => SetValue(SmoothProperty, value);
        }

        public ChromaKeyEffect()
        {
            PixelShader = new PixelShader()
            {
                UriSource = new Uri("pack://application:,,,/Delight.Component;component/Resources/chromakey.ps")
            };

            float t = 0.1F;

            float h = 120;
            float s = 0.963F;
            float l = 0.429F;

            HueMin = ClipValue(h - (360 * t), 0, 360);
            HueMax = ClipValue(h + (360 * t), 0, 360);

            SaturationMin = ClipValue(s - t, 0, 1);
            SaturationMax = ClipValue(s + t, 0, 1);

            LuminanceMin = ClipValue(l - t, 0, 1);
            LuminanceMax = ClipValue(l + t, 0, 1);



            UpdateShaderValue(BrushProperty);
            UpdateShaderValue(HueMinProperty);
            UpdateShaderValue(HueMaxProperty);
            UpdateShaderValue(SaturationMinProperty);
            UpdateShaderValue(SaturationMaxProperty);
            UpdateShaderValue(LuminanceMinProperty);
            UpdateShaderValue(LuminanceMaxProperty);
            UpdateShaderValue(SmoothProperty);
        }

        public void SetColor(Color color, float threshHold)
        {

        }

        public float ClipValue(float input, float min, float max)
        {
            if (input < min)
                return min;

            if (max < input)
                return max;

            return input;
        }
    }
}
