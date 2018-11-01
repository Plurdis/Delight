using Delight.Core.Stage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Core.Attributes
{
    /// <summary>
    /// 출력되는 장치를 나타내는 특성입니다.
    /// </summary>
    public class OutputDeviceAttribute : Attribute
    {
        public OutputDeviceAttribute(OutputDevice outputDevice)
        {
            OutputDevice = outputDevice;
        }

        public OutputDevice OutputDevice { get; }
    }
}
