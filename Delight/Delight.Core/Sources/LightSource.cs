﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Delight.Core.Sources.Options;

namespace Delight.Core.Sources
{
    public class LightSource : BaseSource
    {
        public LightSource() : base("조명 아이템")
        {
            ThumbnailUri = "SpecialURI:MovingLight";
        }

        public override List<BaseOption> Options => new List<BaseOption>() { new GenericOption() { Name = "다운로드", Tag = "download" } };

        public string SourceName { get; set; }

        public override void Download(int SelectedIndex)
        {
            throw new Exception("해당 효과는 아직 템플릿에서만 접근 할 수 있습니다. Comming Soon!");
        }
    }
}
