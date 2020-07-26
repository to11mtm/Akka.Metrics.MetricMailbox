using Akka.Actor;
using Akka.Configuration;

namespace Akka.Metrics.MetricMailbox
{
    public static class MetricMailboxExtensions
    {
        public static Props WithDefaultMetricMailbox(this Props props)
        {
            return props.WithMailbox("metric-mailbox");
        }
        public static Config WithDefaultMetricMailbox(this Config config)
        {
            return MetricMailboxConfig.WithMetricMailbox(config);
        }
    }
}