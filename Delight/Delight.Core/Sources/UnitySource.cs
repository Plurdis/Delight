using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Delight.Core.Sources.Options;

namespace Delight.Core.Sources
{
    public class UnitySource : BaseSource
    {
        public UnitySource() : base("유니티 효과")
        {
        }

        public UnitySource(string title, string thumbnailUri) : this()
        {
            Title = title;
            ThumbnailUri = thumbnailUri;
        }

        public override List<BaseOption> Options => new List<BaseOption>() { new GenericOption() { Name = "다운로드" } };

        public string DownloadLink { get; set; }

        public string Data { get; set; }

        public override bool Download(int SelectedIndex)
        {
            // TODO: 다운로드 추가
            return true;
        }

        public override TemplateData GetTemplateData()
        {
            return new TemplateData()
            {
                FileName = Title,
                StreamUse = true,
                Id = Id,
                // TODO: Stream 추가
            };
        }
    }
}
