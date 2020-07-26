using System;
using Akka.Actor;
using Akka.Dispatch;
using Akka.Dispatch.MessageQueues;
using Akka.Mailbox.Visualizer;

namespace Akka.Metrics.MetricMailbox
{
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
                DateTime.UtcNow.Ticks/TimeSpan.TicksPerMillisecond );
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