using System;
using Akka.Mailbox.Visualizer;
using StackExchange.Redis;

namespace Akka.Metrics.MetricMailbox.RedisPubSubListener
{
    public class ObservableMetricSubscriber : MetricSubscriberAbstraction
    {
        public IObservable<MailboxMetric> ObservableSet { get; protected set; }

        public ObservableMetricSubscriber(
            IConnectionMultiplexer connectionMultiplexer,
            IMetricSerializer serializer, string keyName) : base(
            connectionMultiplexer, serializer, keyName)
        {
            ObservableSet =
                _subscriber.WhenMessageReceived(_channel, _serializer);
        }

    }
}