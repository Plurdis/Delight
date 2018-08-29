using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Common
{
    public static class DebugHelper
    {
        static bool AllowOutput = false;

        public static void WriteLine(string message)
        {
#if DEBUG
            if (AllowOutput)
                Console.WriteLine(message);
#endif
        }

        public static void WriteLine(bool message)
        {
#if DEBUG
            if (AllowOutput)
                Console.WriteLine(message);
#endif
        }

        public static void WriteLine(double message)
        {
#if DEBUG
            if (AllowOutput)
                Console.WriteLine(message);
#endif
        }

        public static void WriteLine(float message)
        {
#if DEBUG
            if (AllowOutput)
                Console.WriteLine(message);
#endif
        }
    }
}
