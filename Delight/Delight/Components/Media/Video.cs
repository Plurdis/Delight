using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Components.Media
{
    /// <summary>
    /// 사용하는 여러가지 미디어 형식 중에 비디오를 나타냅니다.
    /// </summary>
    public class Video : IMedia
    {
        public string Identity { get; set; }

        public TimeSpan Duration { get; set; }

        public string FileLocation { get; set; }

    }
}
