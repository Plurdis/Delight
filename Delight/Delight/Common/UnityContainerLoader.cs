using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

using WinFormControl = System.Windows.Forms.Control;

namespace Delight.Common
{
    public class UnityContainerLoader
    {
        #region [  Interop Members  ]

        [DllImport("User32.dll")]
        static extern bool MoveWindow(IntPtr handle, int x, int y, int width, int height, bool redraw);

        internal delegate int WindowEnumProc(IntPtr hwnd, IntPtr lparam);
        [DllImport("user32.dll")]
        internal static extern bool EnumChildWindows(IntPtr hwnd, WindowEnumProc func, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern int SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private Process process;
        private IntPtr unityHWND = IntPtr.Zero;

        private const int WM_ACTIVATE = 0x0006;
        private readonly IntPtr WA_ACTIVE = new IntPtr(1);
        private readonly IntPtr WA_INACTIVE = new IntPtr(0);

        #endregion

        public UnityContainerLoader(string fileName, Window parent, WinFormControl winformControl)
        {
            control = winformControl;

            parent.SizeChanged += Parent_SizeChanged;

            MainWindow mw = Application.Current.MainWindow as MainWindow;
            mw.Closing += Parent_Closing;
            parent.Activated += Parent_Activated;
            parent.Deactivated += Parent_Deactivated;
            process = new Process();
            process.StartInfo.FileName = fileName;
            
            process.StartInfo.Arguments = $"-parentHWND {winformControl.Handle} {Environment.NewLine}";
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.CreateNoWindow = true;
            //process.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;

            process.Start();

            process.WaitForInputIdle();
            //unityHWND = process.MainWindowHandle;
            EnumChildWindows(winformControl.Handle, WindowEnum, IntPtr.Zero);

            ActivateUnityWindow();
        }

        private void Parent_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                process.CloseMainWindow();

                Thread.Sleep(1000);
                while (!process.HasExited)
                    process.Kill();
            }
            catch (Exception)
            {
            }
        }

        private void Parent_Deactivated(object sender, EventArgs e)
        {
            DeactivateUnityWindow();
        }

        private void Parent_Activated(object sender, EventArgs e)
        {
            ActivateUnityWindow();
        }

        private void Parent_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            MoveWindow(unityHWND, 0, 0, control.Width, control.Height, true);
            ActivateUnityWindow();
        }

        WinFormControl control;

        private void ActivateUnityWindow()
        {
            SendMessage(unityHWND, WM_ACTIVATE, WA_ACTIVE, IntPtr.Zero);
        }

        private void DeactivateUnityWindow()
        {
            SendMessage(unityHWND, WM_ACTIVATE, WA_INACTIVE, IntPtr.Zero);
        }

        private int WindowEnum(IntPtr hwnd, IntPtr lparam)
        {
            unityHWND = hwnd;
            ActivateUnityWindow();
            return 0;
        }
    }
}
