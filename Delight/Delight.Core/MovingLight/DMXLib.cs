using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Core.MovingLight
{
    internal static class DMXLib
    {
        const string dllName = "USB-DMX.dll";

        [DllImport(dllName, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        static extern int DMXSend(int Channel, byte Value);

        [DllImport(dllName, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        static extern int DMXSends(int ChannelCount, int ChannelIndex, byte[] Value);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        static extern bool DMXOpen();

        [DllImport(dllName, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        static extern void DMXClose();

        public static int Send(int channel, byte value) => DMXSend(channel, value);

        public static int Sends(int channel, int index, byte[] value) => DMXSends(channel, index, value);

        public static void Open()
        {
            Task.Factory.StartNew(() =>
            {
                bool open = DMXOpen();
            });
        }

        public static void Close() => DMXClose();
    }
}
