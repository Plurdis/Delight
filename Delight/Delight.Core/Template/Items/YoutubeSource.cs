using System;
using System.Collections.Generic;
using Delight.Core.Template.Options;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Delight.Core.Common;

namespace Delight.Core.Template.Items
{
    public class YoutubeSource : BaseSource
    {
        public string LowDownloadUrl { get; set; }

        public string NormalDownloadUrl { get; set; }

        public string HighDownloadUrl { get; set; }

        public override List<BaseOption> Options { get; set; }

        public YoutubeSource(string title, string thumbnailUri)
        {
            Title = title;
            ThumbnailUri = thumbnailUri;
        }

        public YoutubeSource()
        {

        }

        public override void Download()
        {
            Console.WriteLine("Download Required");
        }
    }
}
