using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Common
{
    public static class DebugHelper
    {
        public static void WriteLine(string message)
        {
#if DEBUG
            Console.WriteLine(message);
#endif
        }
    }
}
