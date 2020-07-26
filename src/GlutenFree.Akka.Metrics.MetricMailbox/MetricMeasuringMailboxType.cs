using Akka.Actor;
using Akka.Configuration;
using Akka.Dispatch;
using Akka.Dispatch.MessageQueues;

namespace GlutenFree.Akka.Metrics.MetricMailbox
{
    public class MetricMeasuringMailboxType : MailboxType, IProducesMessageQueue<MetricMeasuringMailbox>
    {
        public MetricMeasuringMailboxType() : this(null, null)
        {
            
        }
        public MetricMeasuringMailboxType(Settings settings, Config config) : base(settings,config)
        {
            
        }

        public override IMessageQueue Create(IActorRef owner, ActorSystem system)
        {
            return new MetricMeasuringMailbox(new UnboundedMailbox(Settings,Config).Create(owner,system),system);
        }
    }
}