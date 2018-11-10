using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Core.Stage.Components.Media
{
    public class SoundMedia : BaseMedia
    {
        public SoundMedia() : base(SourceType.Sound, false)
        {
        }

        public override string TypeText => "사운드";
    }
}
