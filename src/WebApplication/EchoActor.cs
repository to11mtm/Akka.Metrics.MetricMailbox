using System;
using System.Threading;
using Akka.Actor;

namespace WebApplication
{
    public class EchoActor : ActorBase
    {
        private Random _rand;

        public EchoActor()
        {
            _rand = new Random();
        }
        protected override bool Receive(object message)
        {
            if (message is Echo)
            {
                SpinWait.SpinUntil(() => false,
                    //TimeSpan.FromSeconds(_rand.NextDouble()*3));
                    TimeSpan.FromMilliseconds(_rand.Next(5, 200)));
                Context.Sender.Tell(message);
                return true;
            }

            return false;
        }
    }
}