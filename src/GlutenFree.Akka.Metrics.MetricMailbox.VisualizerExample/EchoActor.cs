using System;
using System.Threading;
using Akka.Actor;

namespace GlutenFree.Akka.Metrics.MetricMailbox.VisualizerExample
{
    public class EchoActor : ActorBase
    {
        private Random _rand;
        private bool _fast;

        public EchoActor(bool isFast)
        {
            _fast = isFast;
            _rand = new Random();
        }
        protected override bool Receive(object message)
        {
            if (message is Echo)
            {
                if (_fast)
                {
                    SpinWait.SpinUntil(() => false,
                    TimeSpan.FromMilliseconds(_rand.Next(5, 200)));
                }
                else
                {
                    SpinWait.SpinUntil(() => false,
                        TimeSpan.FromSeconds(_rand.NextDouble()*3));
                }
                
                    
                Context.Sender.Tell(message);
                return true;
            }

            return false;
        }
    }
}