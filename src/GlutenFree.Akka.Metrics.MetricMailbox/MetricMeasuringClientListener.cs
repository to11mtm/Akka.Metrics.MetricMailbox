using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Event;

namespace GlutenFree.Akka.Metrics.MetricMailbox
{
    public class MetricMeasuringClientListener : ActorBase
    {
        private int _bufferSize;
        public MetricMeasuringClientListener(IMetricMeasuringWriter writer, int bufferSize = 40,
            int autoFlushAfterSeconds = 1)
        {
            _writer = writer;
            _bufferSize = bufferSize;
            MetricBuffer = new List<MailboxMetric>(_bufferSize);
            Context.System.EventStream.Subscribe(Context.Self,
                typeof(MailboxMetric));
            _cancelToken   = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(
                TimeSpan.FromSeconds(autoFlushAfterSeconds),
                TimeSpan.FromSeconds(autoFlushAfterSeconds), Context.Self,
                Flush.Instance, Context.Self);
        }
        private List<MailboxMetric> MetricBuffer;
        private IMetricMeasuringWriter _writer;
        private ICancelable _cancelToken;

        protected override void PostStop()
        {
            _cancelToken?.Cancel();
            Context.System.EventStream.Unsubscribe(Context.Self);
            base.PostStop();
        }

        protected override bool Receive(object message)
        {
            if (message is MailboxMetric vmm)
            {
                MetricBuffer.Add(vmm);
                if (MetricBuffer.Count > _bufferSize)
                {
                    Context.Self.Tell(Flush.Instance);
                }
            }
            else if (message is Flush)
            {
                try
                {
                    _writer.WriteMetrics(MetricBuffer.ToArray());
                    MetricBuffer.Clear();
                }
                catch (Exception e)
                {
                    Context.GetLogger().Error(e,"Error writing Mailbox Metrics!");
                }
            }

            return true;
        }
    }
}