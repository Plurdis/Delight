using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Core.Stage.Components
{
    public class LightComponent : StageComponent
    {
        public LightComponent() : base(SourceType.Light, true)
        {
        }

        public string MovingPreset { get; }
    }
}
