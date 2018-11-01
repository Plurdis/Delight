using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Core.Stage.Components.Media
{
    public abstract class BaseMedia : StageComponent
    {
        public BaseMedia(SourceType sourceType, bool isDynamicLength = false) : base(sourceType, false)
        {
        }

        public string Path { get; set; }
    }
}
