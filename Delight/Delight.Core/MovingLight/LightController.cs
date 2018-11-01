using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Delight.Core.MovingLight
{
    public static class MovingLightControl
    {
        // Constructor
        static MovingLightControl()
        {
            DMXLib.Open();
        }

        public static void Init()
        {
            // Fake Method
        }

        public static void Send(int port, byte value)
        {
            DMXLib.Send(port, value);
        }
    }
}
