using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Core.Stage.Components.Media
{
    public class VideoMedia : BaseMedia
    {
        public VideoMedia() : base(SourceType.Video, false)
        {
        }

        public override string TypeText
        {
            get
            {
                if (FromYoutube)
                    return "유튜브 영상";

                return "동영상";
            }
        }

        public bool FromYoutube { get; set; }

        public string DownloadLink { get; set; }
    }
}
