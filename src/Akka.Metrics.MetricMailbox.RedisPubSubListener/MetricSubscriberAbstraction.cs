using StackExchange.Redis;

namespace Akka.Metrics.MetricMailbox.RedisPubSubListener
{
    public class MetricSubscriberAbstraction
    {
        protected ISubscriber _subscriber;
        protected RedisChannel _channel;
        protected IMetricSerializer _serializer;

        public MetricSubscriberAbstraction(IConnectionMultiplexer connectionMultiplexer, IMetricSerializer serializer, string keyName)
        {
            _subscriber = connectionMultiplexer.GetSubscriber();
            _serializer = serializer;
            _channel = new RedisChannel("akka.metrics.metricmailbox." + keyName,
                RedisChannel.PatternMode.Literal);
            
        }
    }
}