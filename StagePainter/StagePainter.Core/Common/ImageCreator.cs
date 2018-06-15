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
        public static ImageSource GetWireFrame(int width, int height, Brush brush)
        {
            DrawingVisual dv = new DrawingVisual();
            Pen p = new Pen(brush, 0.5);

            using (DrawingContext dc = dv.RenderOpen())
            {
                var glc = new GuidelineSet();
                glc.GuidelinesX.Add(0.5);
                glc.GuidelinesY.Add(0.5);
                glc.GuidelinesX.Add(width - 0.5);
                glc.GuidelinesY.Add(height - 0.5);


                dc.PushGuidelineSet(glc);
                dc.DrawRectangle(null, p, new Rect(0, 0, width - 1, height - 1));

                dc.DrawLine(p, new Point(0, 0), new Point(width, height));
                dc.DrawLine(p, new Point(width, 0), new Point(0, height));
            }

            RenderTargetBitmap bmp = new RenderTargetBitmap(width,height, 96, 96, PixelFormats.Pbgra32);

            bmp.Render(dv);

            return bmp;
        }
    }
}
