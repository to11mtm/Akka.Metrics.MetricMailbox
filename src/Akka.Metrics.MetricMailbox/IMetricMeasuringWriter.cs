namespace Akka.Mailbox.Visualizer
{
    public interface IMetricMeasuringWriter
    {
        void WriteMetrics(MailboxMetric[] metric);

    }
}