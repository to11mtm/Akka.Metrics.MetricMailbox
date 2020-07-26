using Akka.Mailbox.Visualizer;

namespace Akka.Metrics.MetricMailbox.RedisPubSubListener
{
    public class MailboxMetricPayload
    {
        public MailboxMetric[] Set { get; set; }
    
    }
}