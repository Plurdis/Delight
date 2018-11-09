using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Delight.Core.MovingLight.Effects
{
    public class LightBoard
    {
        public LightBoard()
        {
            Storys = new List<BaseState>();
        }

        public void AddState(LightState lightState)
        {
            Storys.Add(lightState);
        }

        public void AddDelayState(DelayState delayState)
        {
            Storys.Add(delayState);
        }

        public void AddWait(int milliseconds)
        {
            Storys.Add(new WaitState(milliseconds));
        }

        [XmlElement(typeof(DelayState))]
        [XmlElement(typeof(LightState))]
        [XmlElement(typeof(WaitState))]
        public List<BaseState> Storys { get; set; }

        public string Identifier { get; set; }

        public TimeSpan Length
        {
            get
            {
                long milliseconds = 0;
                foreach (BaseState state in Storys)
                {
                    if (state is DelayState ds)
                    {
                        milliseconds += ds.DelayTiming;
                    }
                    else if (state is WaitState ws)
                    {
                        milliseconds += ws.MilliSeconds;
                    }
                }

                return TimeSpan.FromMilliseconds(milliseconds);
            }
        }

        Thread thr;

        int _startPort;
    }
}
