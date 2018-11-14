using Delight.Component.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Delight.Component.KeyFrames.ChromaKey
{
    public class ChromaKeyFrame : BaseKeyFrame
    {
        public ChromaKeyFrame(ChromaKeyEffect targetEffect) : base(targetEffect)
        {
        }

        public new ChromaKeyEffect TargetEffect => (ChromaKeyEffect)base.TargetEffect;

        public Color CurrentColor { get; set; }

        public double Usage { get; set; }
    }
}
