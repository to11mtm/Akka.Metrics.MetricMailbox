using Akka.Mailbox.Visualizer;
using Akka.Streams.Dsl;
using StackExchange.Redis;

namespace Akka.Metrics.MetricMailbox.RedisPubSubListener
{
    public class MetricPublisher : IMetricMeasuringWriter
    {
        private ISubscriber _subscriber;
        private RedisChannel _channel;
        private IMetricSerializer _serializer;

        public MetricPublisher(IConnectionMultiplexer connectionMultiplexer, IMetricSerializer serializer, string keyName)
        {
            _subscriber = connectionMultiplexer.GetSubscriber();
            _serializer = serializer;
            _channel = new RedisChannel("akka.metrics.metricmailbox." + keyName,
                RedisChannel.PatternMode.Literal);
        }

        public void PublishMetrics(MailboxMetric[] metricSet)
        {
            _subscriber.Publish(_channel,
                _serializer.GetSerializedBytes(new MailboxMetricPayload()
                    {Set = metricSet}));
        }

        public void WriteMetrics(MailboxMetric[] metric)
        {
            PublishMetrics(metric);
        }
    }
}