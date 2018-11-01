using Delight.Core.Common;
using Delight.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Delight.Core.Timers
{
    /// <summary>
    /// 프레임 단위로 지정된 시간마다 Tick을 발생시킵니다.
    /// </summary>
    public class FrameTimer
    {
        public delegate void FrameTickDelegate(FrameTimer timer, EventArgs e);
        public event FrameTickDelegate Tick;

        public FrameRate FrameRate { get; set; }

        public bool IsRunning => _sw.IsRunning;

        private Stopwatch _sw;
        private Thread _thr;
        private int _savedFrame;

        public FrameTimer(FrameRate frameRate)
        {
            _sw = new Stopwatch();
            FrameRate = frameRate;
        }

        public void Start()
        {
            _sw.Restart();

            _thr = new Thread(() =>
            {
                int lastElapsed = -1;
                while (IsRunning)
                {
                    int elapsed = ((int)Math.Truncate(_sw.ElapsedMilliseconds / 1000.0) * 60)
                                    + (int)((_sw.ElapsedMilliseconds % 1000) / (1000.0 / FrameRate.ToInt32()));
                    if (lastElapsed != elapsed)
                    {
                        int diff = (elapsed - lastElapsed);
                        _savedFrame += diff;
                        while (_savedFrame > 0)
                        {
                            Tick?.Invoke(this, new EventArgs());
                            _savedFrame--;
                        }
                    }
                    lastElapsed = elapsed;

                    Thread.Sleep(1);
                }
            });

            _thr.Start();
        }

        public void Stop()
        {
            _sw.Stop();
        }
    }
}
