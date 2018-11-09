using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Delight.Core.MovingLight
{
    public class DMXController
    {
        public static void Reset()
        {
            Task.Run(() =>
            {
                for (int i = 1; i <= 512; i++)
                {
                    DMXLib.Send(i, 1);
                    Thread.Sleep(1);
                }
            });
        }

        // Constructor
        static DMXController()
        {
            DMXLib.Open();
        }

        public static void Init()
        {
            // Fake Method
        }

        /// <summary>
        /// <see cref="DMXController"/> 클래스를 초기화합니다.
        /// </summary>
        /// <param name="startPort">포트의 시작점을 나타냅니다. 1부터 512 사이에서 설정할 수 있습니다.</param>
        public DMXController(int startPort)
        {
            if (startPort < 1)
            {
                throw new IndexOutOfRangeException("범위 값을 벗어났습니다. startPort의 값은 1보다 작을 수 없습니다.");
            }
            else if (startPort > 512)
            {
                throw new IndexOutOfRangeException("범위 값을 벗어났습니다. startPort의 값은 512보다 클 수 없습니다.");
            }

            _startPort = startPort;
        }

        private int _startPort;

        public int StartPort
        {
            get
            {
                return _startPort;
            }
            set
            {
                if (value < 1)
                    value = 1;

                if (value > 512)
                    value = 512;

                _startPort = value;
            }
        }

        /// <summary>
        /// 해당 포트 번호에 해당하는 포트의 값을 전송합니다.
        /// </summary>
        /// <param name="portNumber">포트 번호입니다. 12번까지 사용가능합니다.</param>
        /// <param name="value"></param>
        public bool Send(PortNumber portNumber, byte value)
        {
            if ((int)portNumber < 0 || (int)portNumber > 12)
            {
                return false;
            }
            try
            {
                // TODO: 512를 시작점으로 잡았을때 offset 값을 설정해줘야 함

                DMXLib.Send(_startPort - 1 + (int)portNumber, value);
                Thread.Sleep(1);
                //Console.WriteLine($"{portNumber}에 {value}값 전달");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
