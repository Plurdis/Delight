using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Delight.Timing;

namespace Delight.Common.Components
{
    public class Effect : StageComponent
    {
        public Effect() : base(TrackType.Effect,false)
        {
        }
    }
}
