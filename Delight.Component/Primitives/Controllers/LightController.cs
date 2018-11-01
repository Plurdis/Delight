using Delight.Component.Controls;
using Delight.Component.Extensions;
using Delight.Component.Primitives.TimingReaders;
using Delight.Core.MovingLight;
using Delight.Core.Stage.Components;
using System;
using System.Threading;

namespace Delight.Component.Primitives.Controllers
{
    public class LightController : BaseController
    {
        public LightController(Track track, TimingReader reader) : base(track, reader, false)
        {
        }

        LightMovement _effect;

        public override void ItemStarted(TrackItem sender, TimingEventArgs e)
        {
            string movingPreset = (sender.GetTag<LightComponent>().MovingPreset);

            //switch (movingPreset.ToLower())
            //{
            //    case "movetox":
            //        _effect = new LightXEffect();
            //        break;
            //    case "movetoy":
            //        _effect = new LightYEffect();
            //        break;
            //    default:
            //        break;
            //}
            
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
                        MovingLightControl.Send(2, 70);
                    }
                    else
                    {
                        Console.WriteLine("Y Effect - To 180");
                        MovingLightControl.Send(2, 180);
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
                        MovingLightControl.Send(1, 1);
                    }
                    else
                    {
                        Console.WriteLine("X Effect - To 180");
                        MovingLightControl.Send(1, 180);
                    }
                    Thread.Sleep(SleepTime);
                    _zero = !_zero;
                }
            });

            thr.Start();
        }
    }
}
