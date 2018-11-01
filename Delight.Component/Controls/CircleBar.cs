using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Delight.Component.Controls
{
    [TemplatePart(Name = "piece", Type = typeof(PiePiece))]
    public class CircleBar : Control
    {
        public CircleBar()
        {
            this.Style = FindResource("CircleBarStyle") as Style;
        }

        PiePiece piece;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            piece = GetTemplateChild("piece") as PiePiece;
        }

        public static DependencyProperty InnerValueProperty = DependencyProperty.Register(nameof(InnerValue),
            typeof(double),
            typeof(CircleBar),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));

        public double InnerValue
        {
            get => (double)GetValue(InnerValueProperty);
            set => SetValue(InnerValueProperty, value);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            piece.WedgeAngle = InnerValue % 360;
        }
    }
}
