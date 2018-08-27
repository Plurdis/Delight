using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

using Delight.Core.Common;
using Delight.Core.Extension;

namespace Delight.Timing
{
    public class FrameTimer
    {
        public delegate void EmptyDelegate();
        public event EmptyDelegate Tick;

        Stopwatch sw;
        public FrameRate FrameRate { get; set; }

        int FrameRateInt => (int)(FrameRate.GetEnumAttribute<DefaultValueAttribute>().Value);

        public FrameTimer(FrameRate frameRate)
        {
            sw = new Stopwatch();
            FrameRate = frameRate;
        }

        Thread thr;

        public bool IsRunning => sw.IsRunning;

        public void Start()
        {
            sw.Restart();

            thr = new Thread(() =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                int lastElap = -1;
                while (IsRunning)
                {
                    int elapsed = ((int)Math.Truncate(sw.ElapsedMilliseconds / 1000.0) * 60)
                                    + (int)((sw.ElapsedMilliseconds % 1000) / (1000.0 / FrameRateInt));
                    if (lastElap != elapsed)
                    {
                        int diff = (elapsed - lastElap);
                        i += diff;
                        while (i > 0)
                        {
                            Tick?.Invoke();
                            i--;
                        }
                    }
                    lastElap = elapsed;

                    Thread.Sleep(1);
                }
            });
            thr.Start();
        }

        int i = 0;

        public void Stop()
        {
            sw.Stop();
        }
    }
}
