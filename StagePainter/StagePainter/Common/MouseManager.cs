using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StagePainter.Common
{
    public static class MouseManager
    {
        static MouseManager()
        {
            Event = Hook.GlobalEvents();

            Event.MouseDown += Event_MouseDown;
            Event.MouseUp += Event_MouseUp;
        }

        static IMouseEvents Event;

        public static void Init()
        {
        }

        public static bool IsMouseDown { get; private set; }

        public static Point MousePosition => Control.MousePosition;

        #region [  Added Event  ]

        private static void Event_MouseUp(object sender, MouseEventArgs e)
        {
            IsMouseDown = false;
        }

        private static void Event_MouseDown(object sender, MouseEventArgs e)
        {
            IsMouseDown = true;
        }

        #endregion
    }
}
