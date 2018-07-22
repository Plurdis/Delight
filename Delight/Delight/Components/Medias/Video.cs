using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Components.Medias
{
    /// <summary>
    /// 사용하는 여러가지 미디어 형식 중에 비디오를 나타냅니다.
    /// </summary>
    public class Video : Media
    {
        public int VideoHeight { get; set; }
        public int VideoWidth { get; set; }
    }
}
