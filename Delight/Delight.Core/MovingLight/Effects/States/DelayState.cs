using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Core.MovingLight.Effects
{
    /// <summary>
    /// 기존 상태에서 지정된 상태로 지정된 시간만큼 이동합니다.
    /// </summary>
    public class DelayState : BaseState
    {
        public DelayState(int delayTiming, params byte[] values) : this()
        {
            DelayTiming = delayTiming;

            int i = 0;
            foreach (byte b in values)
            {
                Values[i++] = b;
            }
        }

        public DelayState()
        {
            Values = new byte[12];
        }

        public byte[] Values { get; set; }

        public int DelayTiming { get; set; }
    }
}
