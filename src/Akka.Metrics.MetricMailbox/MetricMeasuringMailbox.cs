using System;
using Akka.Actor;
using Akka.Configuration;
using Akka.Dispatch;
using Akka.Dispatch.MessageQueues;
using Akka.Mailbox.Visualizer;

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
    public static class MetricMailboxConfig
    {
        public static Config Config()
        {
            return ConfigurationFactory.ParseString(
                DefaultMetricMailboxConfigString);
        }
        public static Config WithMetricMailbox(Config config)
        {
            return config.WithFallback(
                Config());
        }
        public static string DefaultMetricMailboxConfigString = @"metric-mailbox {
      mailbox-type = ""Akka.Metrics.MetricMailbox.MetricMeasuringMailboxType, Akka.Metrics.MetricMailbox""
 
      # If the mailbox is bounded then this is the timeout for enqueueing
      # in case the mailbox is full. Negative values signify infinite
      # timeout, which should be avoided as it bears the risk of dead-lock.
        mailbox-push-timeout-time = 10s
 
      # For Actor with Stash: The default capacity of the stash.
      # If negative (or zero) then an unbounded stash is used (default)
      # If positive then a bounded stash is used and the capacity is set using
      # the property
            stash-capacity = -1
    }";
    }

    public class MetricMeasuringMailbox : IMessageQueue
    {
        private IMessageQueue _backend;
        private ActorSystem _system;
        public MetricMeasuringMailbox(IMessageQueue backend,
            ActorSystem system)
        {
            _backend = backend;
            _system = system;
        }
        public void Enqueue(IActorRef receiver, Envelope envelope)
        {
            var metric = new MailboxMetric(
                envelope.Sender.Path.ToSerializationFormat(),
                receiver.Path.ToSerializationFormat(), _backend.Count,
                DateTime.UtcNow.Ticks);
            _system.EventStream.Publish(metric);
            _backend.Enqueue(receiver,envelope);
        }

        public bool TryDequeue(out Envelope envelope)
        {
            return _backend.TryDequeue(out envelope);
        }

        public void CleanUp(IActorRef owner, IMessageQueue deadletters)
        {
            _backend.CleanUp(owner,deadletters);
        }

        public bool HasMessages => _backend.HasMessages;
        public int Count => _backend.Count;
    }
}