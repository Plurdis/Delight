using Delight.Component.Controls;
using Delight.Component.Extensions;
using Delight.Component.Primitives.TimingReaders;
using Delight.Core.MovingLight;
using Delight.Core.MovingLight.Effects;
using Delight.Core.Stage.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Delight.Component.Primitives.Controllers
{
    public class LightController : BaseController
    {
        public LightController(Track track, TimingReader reader) : base(track, reader, false)
        {
            _startPort = ((Track.TrackNumber - 1) * 12) + 1;
        }
        
        public override void ItemStarted(TrackItem sender, TimingEventArgs e)
        {
            _states = sender.GetTag<LightComponent>().States;
            thr = new Thread(BeginAction);
            thr.Start();
        }

        public override void ItemEnded(TrackItem sender, TimingEventArgs e)
        {
            Stop();
        }

        public override void ItemPlaying(TrackItem sender, TimingEventArgs e)
        {
        }

        public override void ItemReady(TrackItem sender, TimingReadyEventArgs e)
        {
        }

        public override void TimeLineStopped()
        {
        }

        int _startPort;
        Thread thr;

        IEnumerable<BaseState> _states;

        public void BeginAction()
        {
            DMXController lightController = new DMXController(_startPort);
            var lastStates = new byte[16];
            while (true)
            {
                foreach (BaseState state in _states)
                {
                    if (state is DelayState ds)
                    {
                        var max = ds.DelayTiming / 12;

                        for (int i = 1; i <= max; i++)
                        {
                            PortNumber n = PortNumber.XAxis;
                            foreach (byte value in ds.Values)
                            {
                                int lastState = lastStates[(int)n - 1];
                                int j = value - lastState;

                                int finalValue = (int)((j / (double)max) * i);

                                lightController.SetValue(n++, (byte)(lastState + finalValue));
                                //Thread.Sleep(1);
                                //Console.Write($"{_startPort - 1 + (int)n} : {(byte)(lastState + finalValue)} |");
                            }
                            //Console.WriteLine();
                        }

                        lastStates = ds.Values;
                    }
                    else if (state is LightState ls)
                    {
                        PortNumber n = PortNumber.XAxis;
                        foreach (byte b in ls.States)
                        {
                            lightController.SetValue(n++, b);
                        }

                        lastStates = ls.States;
                    }
                    else if (state is WaitState ws)
                    {
                        Thread.Sleep(ws.MilliSeconds);
                    }
                }
            }
        }

        public void Stop()
        {
            thr?.Abort();
        }
    }
    
}
