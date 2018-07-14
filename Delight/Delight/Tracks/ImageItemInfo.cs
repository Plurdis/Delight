using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Delight.Tracks
{
    /// <summary>
    /// 이미지 트랙 아이템의 정보를 나타냅니다.
    /// </summary>
    public class ImageItemInfo : ITrackItemInfo
    {
        public ImageItemInfo()
        {
        }

        public void LoadFromFile(string fileName)
        {
            Uri uri = new Uri(fileName, UriKind.RelativeOrAbsolute);
            BitmapImage bitmapImage = new BitmapImage(uri);
        }

        public void LoadFromMediaManager(string key)
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
