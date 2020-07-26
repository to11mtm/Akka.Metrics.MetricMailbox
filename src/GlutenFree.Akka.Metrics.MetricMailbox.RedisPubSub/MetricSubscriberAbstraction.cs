using StackExchange.Redis;

namespace GlutenFree.Akka.Metrics.MetricMailbox.RedisPubSubListener
{
    public class MetricSubscriberAbstraction
    {
        protected ISubscriber _subscriber;
        protected RedisChannel _channel;
        protected IMetricPayloadSerializer PayloadSerializer;

        public MetricSubscriberAbstraction(IConnectionMultiplexer connectionMultiplexer, IMetricPayloadSerializer payloadSerializer, string keyName)
        {
            _subscriber = connectionMultiplexer.GetSubscriber();
            PayloadSerializer = payloadSerializer;
            _channel = new RedisChannel("akka.metrics.metricmailbox." + keyName,
                RedisChannel.PatternMode.Literal);
            
        }
    }
}