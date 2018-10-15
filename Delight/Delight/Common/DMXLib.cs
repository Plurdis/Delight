using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Common
{
    class DMXLib : IDisposable
    {
        const string dllName = "USB-DMX.dll";

        
        [DllImport(dllName, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        static extern int DMXSend(int Channel, byte Value);

        [DllImport(dllName, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        static extern int DMXSends(int ChannelCount, int ChannelIndex, byte[] Value);

        [DllImport(dllName, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        static extern bool DMXOpen();

        [DllImport(dllName, CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        static extern void DMXClose();

        public int Send(int channel, byte value) => DMXSend(channel, value);

        public int Sends(int channel, int index, byte[] value) => DMXSends(channel, index, value);

        public bool Open() => DMXOpen();

        public void Close() => DMXClose();

        public void Dispose()
        {
            Close();
        }
    }
}
