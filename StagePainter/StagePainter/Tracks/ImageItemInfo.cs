using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace StagePainter.Tracks
{
    public class ImageItemInfo : ITrackItemInfo
    {
        public ImageItemInfo()
        {

        }

        public void LoadFromFile(string fileName)
        {
            Uri uri = new Uri(fileName, UriKind.RelativeOrAbsolute);
            BitmapImage bitmapImage = new BitmapImage(uri);
            LoadFromImageSource(bitmapImage);
        }

        public void LoadFromImageSource(ImageSource imageSource)
        {

        }

        public double Offset { get; set; }

        public double Size => 100;

        public string Name { get; set; }

        public object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
