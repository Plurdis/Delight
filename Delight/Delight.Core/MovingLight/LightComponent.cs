using Delight.Component.MovingLight.Effects;
using Delight.Core.Common;
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

        public LightComponent(SetterBoard setterBoard) : this()
        {
            SetterBoard = setterBoard;
            Identifier = setterBoard.Identifier;
            Time = TimeSpan.FromSeconds(20);
            Thumbnail = new Uri("pack://application:,,,/Delight;component/Resources/defaultLightImage.png");
            Id = Crc32.GetHashFromString(BoardSerializer.SerializeToString(setterBoard));
        }

        public SetterBoard SetterBoard { get; set; }

        public override string TypeText => "조명 효과";
    }
}
