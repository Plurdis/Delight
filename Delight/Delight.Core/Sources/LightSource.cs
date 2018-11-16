using Delight.Core.Sources.Options;
using System;
using System.Collections.Generic;
using System.IO;

namespace Delight.Core.Sources
{
    public class LightSource : BaseSource
    {
        public LightSource() : base("조명 아이템")
        {
            ThumbnailUri = "SpecialURI:MovingLight";
        }

        public override List<BaseOption> Options => new List<BaseOption>() { new GenericOption() { Name = "다운로드", Tag = "download" } };

        public string MovingData { get; set; }

        public override bool Download(int SelectedIndex)
        {
            throw new Exception("해당 효과는 아직 템플릿에서만 접근 할 수 있습니다. Comming Soon!");
        }

        public override TemplateData GetTemplateData()
        {
            return new TemplateData()
            {
                FileName = Title,
                Id = Id,
                Stream = GenerateStreamFromString(MovingData),
                StreamUse = true,
            };
        }

        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
