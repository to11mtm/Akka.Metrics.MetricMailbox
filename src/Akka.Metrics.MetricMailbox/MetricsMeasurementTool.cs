using System.Runtime.CompilerServices;
using Akka.Actor;

namespace Akka.Mailbox.Visualizer
{
    public static class MetricsMeasurementTool
    {
        public static ConditionalWeakTable<ActorSystem, IActorRef> _registeredInstances = new ConditionalWeakTable<ActorSystem, IActorRef>();

        public static IActorRef RegisterMailboxMetricListener(this ActorSystem system,
            IMetricMeasuringWriter writer, int bufferSize = 40,
            int autoFlushAfterSeconds = 1)
        {
            return _registeredInstances.GetValue(system, key =>
            {
                return key.ActorOf(Props.Create(() =>
                    new MetricMeasuringClientListener(writer, bufferSize,
                        autoFlushAfterSeconds)));
            });
        }

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