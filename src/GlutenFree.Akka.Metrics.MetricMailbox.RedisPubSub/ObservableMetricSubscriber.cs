using System;
using StackExchange.Redis;

namespace GlutenFree.Akka.Metrics.MetricMailbox.RedisPubSubListener
{
    public interface IObservableMailboxMetricProvider
    {
        IObservable<MailboxMetric> ObservableSet { get; }
    }
    public class ObservableMetricSubscriber : MetricSubscriberAbstraction, IObservableMailboxMetricProvider
    {
        public IObservable<MailboxMetric> ObservableSet { get; protected set; }

        public ObservableMetricSubscriber(
            IConnectionMultiplexer connectionMultiplexer,
            IMetricPayloadSerializer payloadSerializer, string keyName) : base(
            connectionMultiplexer, payloadSerializer, keyName)
        {
            ObservableSet =
                _subscriber.WhenMessageReceived(_channel, PayloadSerializer);
        }

    }
}