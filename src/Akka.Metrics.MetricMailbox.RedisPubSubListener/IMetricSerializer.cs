namespace Akka.Metrics.MetricMailbox.RedisPubSubListener
{
    public interface IMetricSerializer
    {
        public byte[] GetSerializedBytes(MailboxMetricPayload payload);
        public MailboxMetricPayload GetObject(byte[] bytes);
    }
}