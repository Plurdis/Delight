using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Delight.Core.Common
{
    /// <summary>
    /// 기본 이미지들을 생성해냅니다.
    /// </summary>
    public static class ImageCreator
    {
        /// <summary>
        /// X자 와이어 프레임 이미지를 생성합니다.
        /// </summary>
        /// <param name="width">와이어 프레임의 넓이입니다.</param>
        /// <param name="height">와이어 프레임의 높이입니다.</param>
        /// <param name="brush">와이어 프레임 선의 색깔입니다.</param>
        /// <returns></returns>
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
