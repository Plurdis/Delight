using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace StagePainter.Core.Common
{
    public static class ImageCreator
    {
        public static ImageSource GetWireFrame(int width, int height, Pen pen)
        {
            DrawingVisual dv = new DrawingVisual();
            DrawingContext dc = dv.RenderOpen();

            dc.DrawRectangle(null, pen, new Rect(0, 0, width, height));

            dc.DrawLine(pen, new Point(0, 0), new Point(width, height));
            dc.DrawLine(pen, new Point(0, width), new Point(height, 0));
            
            dc.Close();

            RenderTargetBitmap bmp = new RenderTargetBitmap(width,height, 96, 96, PixelFormats.Pbgra32);

            bmp.Render(dv);

            return bmp;
        }
    }
}
