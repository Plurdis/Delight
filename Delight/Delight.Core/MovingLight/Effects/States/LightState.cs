using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delight.Core.MovingLight.Effects
{
    /// <summary>
    /// 단일 상태를 나타냅니다.
    /// </summary>
    public class LightState : BaseState
    {
        public LightState()
        {
            States = new byte[12];
        }

        public LightState(params byte[] values) : this()
        {
            int i = 0;
            foreach(byte b in values)
            {
                States[i++] = b;
            }
        }

        public byte[] States { get; set; }

        public byte this[int index]
        {
            get => States[index];
            set => States[index] = value;
        }
    }
}
