using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Core.MovingLight.Effects
{
    public class WaitState : BaseState
    {
        public WaitState(int milliSeconds)
        {
            MilliSeconds = milliSeconds;
        }

        public WaitState()
        {

        }

        public int MilliSeconds { get; set; }
    }
}
