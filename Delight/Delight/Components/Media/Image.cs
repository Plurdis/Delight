using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Delight.Components.Media
{
    /// <summary>
    /// 사용하는 여러가지 미디어 형식 중에 이미지를 나타냅니다.
    /// </summary>
    public class Image : StageComponent, IMedia
    {
        public string Identity { get; set; }

        public string FileLocation { get; set; }

        public ImageSource ImageSource { get; set; }
    }
}
