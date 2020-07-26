namespace GlutenFree.Akka.Metrics.MetricMailbox
{
    public interface IMetricMeasuringWriter
    {
        void WriteMetrics(MailboxMetric[] metric);
    }
}