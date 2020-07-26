using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace GlutenFree.Akka.Metrics.MetricMailbox.RedisPubSubListener
{
    public abstract class BaseAsyncMetricSubscriber : MetricSubscriberAbstraction
    {
        public async Task HandlePublishEvent(RedisChannel channel,
            RedisValue value)
        {
            try
            {
                await HandlePublishedMetricsAsync(PayloadSerializer.GetObject(value));
            }
            catch (Exception e)
            {
                
            }
            
        }

        public abstract Task HandlePublishedMetricsAsync(
            MailboxMetricPayload payload);

        protected BaseAsyncMetricSubscriber(IConnectionMultiplexer connectionMultiplexer, IMetricPayloadSerializer payloadSerializer, string keyName) : base(connectionMultiplexer, payloadSerializer, keyName)
        {
            _subscriber.Subscribe(_channel,
                async (channel, message) =>
                    await HandlePublishEvent(channel, message));
        }
    }
}