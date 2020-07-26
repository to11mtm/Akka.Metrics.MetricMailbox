using StackExchange.Redis;

namespace GlutenFree.Akka.Metrics.MetricMailbox.RedisPubSubListener
{
    public class MetricPublisher : IMetricMeasuringWriter
    {
        private ISubscriber _subscriber;
        private RedisChannel _channel;
        private IMetricPayloadSerializer _payloadSerializer;

        public MetricPublisher(IConnectionMultiplexer connectionMultiplexer, IMetricPayloadSerializer payloadSerializer, string keyName)
        {
            _subscriber = connectionMultiplexer.GetSubscriber();
            _payloadSerializer = payloadSerializer;
            _channel = new RedisChannel("akka.metrics.metricmailbox." + keyName,
                RedisChannel.PatternMode.Literal);
        }

        public void PublishMetrics(MailboxMetric[] metricSet)
        {
            _subscriber.Publish(_channel,
                _payloadSerializer.GetSerializedBytes(new MailboxMetricPayload()
                    {Set = metricSet}));
        }

        public void WriteMetrics(MailboxMetric[] metric)
        {
            PublishMetrics(metric);
        }
    }
}