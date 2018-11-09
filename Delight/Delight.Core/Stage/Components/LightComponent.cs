using Delight.Core.MovingLight.Effects;
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

        public LightComponent(LightBoard lightBoard) : this()
        {
            States = lightBoard.Storys;
            Identifier = lightBoard.Identifier;
            Time = lightBoard.Length;
            Thumbnail = new Uri("pack://application:,,,/Delight;component/Resources/defaultLightImage.png");
        }

        public List<BaseState> States { get; set; }
    }
}
