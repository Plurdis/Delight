using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Delight.Debug.Manager
{
    public static class MouseManager
    {
        static MouseManager()
        {
        }

        public static void Init()
        {
        }

        public static bool IsMouseDown { get; private set; }

        public static Point MousePosition => Control.MousePosition;
    }
}
