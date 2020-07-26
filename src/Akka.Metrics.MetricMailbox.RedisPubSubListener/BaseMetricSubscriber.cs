using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Akka.Metrics.MetricMailbox.RedisPubSubListener
{
    public abstract class BaseMetricSubscriber : MetricSubscriberAbstraction
    {
        

        public async Task HandlePublishEvent(RedisChannel channel,
            RedisValue value)
        {
            try
            {
                await HandlePublishedMetricsAsync(_serializer.GetObject(value));
            }
            catch (Exception e)
            {
                
            }
            
        }

        public abstract Task HandlePublishedMetricsAsync(
            MailboxMetricPayload payload);

        protected BaseMetricSubscriber(IConnectionMultiplexer connectionMultiplexer, IMetricSerializer serializer, string keyName) : base(connectionMultiplexer, serializer, keyName)
        {
            _subscriber.Subscribe(_channel,
                async (channel, message) =>
                    await HandlePublishEvent(channel, message));
        }
    }
}