using Gma.System.MouseKeyHook;
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

namespace StagePainter.Debug.Manager
{
    public static class MouseManager
    {
        static MouseManager()
        {
            Thread thr = new Thread(() =>
            {
                _hookID = SetHook(_proc);
                Application.Run();
                UnhookWindowsHookEx(_hookID);
            });
            thr.Start();
        }

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


        private static LowLevelMouseProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        private static void MouseMessagesEvents(MouseMessages messages)
        {
            switch (messages)
            {
                case MouseMessages.WM_LBUTTONDOWN:
                    Event_MouseDown(null, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0));
                    break;
                case MouseMessages.WM_LBUTTONUP:
                    Event_MouseUp(null, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0));
                    break;
                case MouseMessages.WM_MOUSEMOVE:
                    break;
                case MouseMessages.WM_MOUSEWHEEL:
                    break;
                case MouseMessages.WM_RBUTTONDOWN:
                    Event_MouseDown(null, new MouseEventArgs(MouseButtons.Right, 0, 0, 0, 0));
                    break;
                case MouseMessages.WM_RBUTTONUP:
                    Event_MouseUp(null, new MouseEventArgs(MouseButtons.Right, 0, 0, 0, 0));
                    break;
                default:
                    break;
            }
        }

        #region [  Low-Level Codes  ]

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                MouseMessagesEvents((MouseMessages)wParam);
            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        private static IntPtr SetHook(LowLevelMouseProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        private const int WH_MOUSE_LL = 14;

        private enum MouseMessages
        {
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_MOUSEMOVE = 0x0200,
            WM_MOUSEWHEEL = 0x020A,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        #endregion
    }
}
