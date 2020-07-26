using System.Runtime.CompilerServices;
using Akka.Actor;

namespace GlutenFree.Akka.Metrics.MetricMailbox
{
    public static class MetricsMeasurementPlugin
    {
        public static ConditionalWeakTable<ActorSystem, IActorRef> _registeredInstances = new ConditionalWeakTable<ActorSystem, IActorRef>();

        /// <summary>
        /// Register the standard MailboxMetric Listener
        /// </summary>
        /// <param name="system">Actorsystem this Listener will listen to</param>
        /// <param name="writer">The writer instance that will receive batches of metrics.</param>
        /// <param name="bufferSize">number of messages to hold before flushing metrics buffer.</param>
        /// <param name="autoFlushAfterSeconds">Metrics Buffer will be flushed regardless of size after this many seconds</param>
        /// <param name="dispatcher">The Dispatcher configuration to use for the listener. If unspecified, the default dispatcher is used.</param>
        /// <returns></returns>
        public static IActorRef RegisterMailboxMetricListener(this ActorSystem system,
            IMetricMeasuringWriter writer, int bufferSize = 40,
            int autoFlushAfterSeconds = 1, string dispatcher = null)
        {
            return _registeredInstances.GetValue(system, key =>
            {
                var props = Props.Create(() =>
                    new MetricMeasuringClientListener(writer, bufferSize,
                        autoFlushAfterSeconds));
                if (string.IsNullOrWhiteSpace(dispatcher))
                {
                    props = props.WithDispatcher(dispatcher);
                }
                return key.ActorOf(props,"mailboxMetricListener");
            });
        }

        //Stops the Registered Mailbox Metric listener for the ActorSystem
        public static void StopMailboxMetricListener(
            this ActorSystem system)
        {
            if (_registeredInstances.TryGetValue(system, out IActorRef toStop))
            {
                toStop.Tell(PoisonPill.Instance);
            }
        }
        
    }
}