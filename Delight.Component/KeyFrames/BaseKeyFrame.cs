using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Effects;

namespace Delight.Component.KeyFrames
{
    public abstract class BaseKeyFrame
    {
        public BaseKeyFrame(ShaderEffect shaderEffect)
        {
        }

        public ShaderEffect TargetEffect { get; set; }

        public int Frame { get; set; }
    }
}
