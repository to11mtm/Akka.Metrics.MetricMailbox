using Akka.Mailbox.Visualizer;
using Akka.Streams.Actors;

namespace Akka.Metrics.MetricMailbox.RedisPubSubListener
{
    public class StreamSubscribePublisher : ActorPublisher<MailboxMetric>
    {
        protected override bool Receive(object message)
        {
            if (message is MailboxMetric m)
            {
                OnNext(m);
                return true;
            }

            return false;
        }
    }
}