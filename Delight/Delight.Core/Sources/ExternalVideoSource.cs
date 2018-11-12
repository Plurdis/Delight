using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Delight.Core.Sources.Options;

namespace Delight.Core.Sources
{
    class ExternalVideoSource : BaseSource
    {
        public ExternalVideoSource() : base("외부 영상")
        {
        }

        public override List<BaseOption> Options => new List<BaseOption>() { new GenericOption()
        {
            Name = "다운로드 불가능",
            Tag = "no_download",
        } };

        public string FileName { get; set; }
        
        public override void Download(int SelectedIndex)
        {
            throw new Exception("해당 영상은 템플릿에서만 접근 할 수 있습니다.");
        }
    }
}
