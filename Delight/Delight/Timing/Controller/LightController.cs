using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Delight.Common;
using Delight.Components;
using Delight.Controls;
using Delight.Extensions;

namespace Delight.Timing.Controller
{
    public class LightController : BaseController
    {
        public LightController(Track track, TimingReader reader) : base(track, reader, false)
        {
        }

        LightMovement _effect;

        public override void ItemStarted(TrackItem sender, TimingEventArgs e)
        {
            string movingPreset = (sender.GetTag<MovingLight>().MovingPreset);

            switch (movingPreset.ToLower())
            {
                case "movetox":
                    _effect = new LightXEffect();
                    break;
                case "movetoy":
                    _effect = new LightYEffect();
                    break;
                default:
                    break;
            }

            

            sender.ItemProperty.PropertyChanged += (sen, ev) =>
            {
                switch (ev.ChangedProperty.ToLower())
                {
                    case "lightfast":
                        _effect.SleepTime = sender.ItemProperty.LightFast;
                        break;
                }
            };

            _effect.Start();
        }

        public override void ItemEnded(TrackItem sender, TimingEventArgs e)
        {
            _effect.Stop();
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
    }

    public abstract class LightMovement
    {
        public int SleepTime { get; set; } = 500;

        internal bool _run = false;

        public abstract void StartEffect();

        public void Start()
        {
            _run = true;
            StartEffect();
        }

        public void Stop()
        {
            _run = false;
        }
    }

    public class LightYEffect : LightMovement
    {
        public override void StartEffect()
        {
            Thread thr = new Thread(() =>
            {
                bool _zero = false;

                while (true)
                {
                    if (!_run)
                        break;
                    if (_zero)
                    {
                        Console.WriteLine("Y Effect - To 70");
                        LightControl.Control((2, 70));
                    }
                    else
                    {
                        Console.WriteLine("Y Effect - To 180"); 
                        LightControl.Control((2, 180));
                    }
                    Thread.Sleep(SleepTime);
                    _zero = !_zero;
                }
            });

            thr.Start();
        }
    }
    public class LightXEffect : LightMovement
    {
        public override void StartEffect()
        {
            Thread thr = new Thread(() =>
            {
                bool _zero = false;

                while (true)
                {
                    if (!_run)
                        break;
                    if (_zero)
                    {
                        Console.WriteLine("X Effect - To 1");
                        LightControl.Control((1, 1));
                    }
                    else
                    {
                        Console.WriteLine("X Effect - To 180");
                        LightControl.Control((1, 180));
                    }
                    Thread.Sleep(SleepTime);
                    _zero = !_zero;
                }
            });

            thr.Start();
        }
    }
}
