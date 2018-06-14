using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace StagePainter.Controls
{
    public class CircleBar : Control
    {
        private static DependencyProperty InnerValueProperty = InnerValuePropertyKey.DependencyProperty;
        public static readonly DependencyPropertyKey InnerValuePropertyKey = DependencyProperty.RegisterReadOnly(nameof(InnerValue), typeof(double), typeof(CircleBar), new PropertyMetadata(default(double)));
        public double InnerValue
        {
            get => (double)GetValue(InnerValueProperty);
            private set => SetValue(InnerValuePropertyKey, value);
        }
    }
}
