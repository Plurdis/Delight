using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Delight.Windows
{
    /// <summary>
    /// TutorialWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TutorialWindow : Window
    {
        public TutorialWindow()
        {
            InitializeComponent();
            shape.MouseLeftButtonDown += Shape_MouseLeftButtonDown;
            shape.MouseMove += Shape_MouseMove;
            shape.MouseLeftButtonUp += Shape_MouseLeftButtonUp;
        }

        private void Shape_MouseMove(object sender, MouseEventArgs e)
        {
            if (captured)
            {
                double x = e.GetPosition(rootElement).X;
                double y = e.GetPosition(rootElement).Y;
                x_shape += x - x_canvas;
                SetLeft(source, x_shape);
                x_canvas = x;
                y_shape += y - y_canvas;
                SetTop(source, y_shape);
                y_canvas = y;
            }
        }

        bool captured = false;
        double x_shape, x_canvas, y_shape, y_canvas;
        UIElement source = null;

        public double GetLeft(object element)
        {
            if (element is FrameworkElement ui)
                return ui.Margin.Left;

            return double.MinValue;
        }
        public double GetTop(object element)
        {
            if (element is FrameworkElement ui)
                return ui.Margin.Top;

            return double.MinValue;
        }
        public void SetLeft(object element, double left)
        {
            if (element is FrameworkElement ui)
                ui.Margin = new Thickness(left, ui.Margin.Top, ui.Margin.Right, ui.Margin.Bottom);
        }
        public void SetTop(object element, double top)
        {
            if (element is FrameworkElement ui)
                ui.Margin = new Thickness(ui.Margin.Left, top, ui.Margin.Right, ui.Margin.Bottom);
        }


        private void Shape_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            source = (UIElement)sender;
            Mouse.Capture(source);
            captured = true;
            x_shape = GetLeft(source);
            x_canvas = e.GetPosition(rootElement).X;
            y_shape = GetTop(source);
            y_canvas = e.GetPosition(rootElement).Y;
        }
        private void Shape_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            captured = false;
        }
    }
}
