namespace GlutenFree.Akka.Metrics.MetricMailbox.RedisPubSubListener
{
    public interface IMetricPayloadSerializer
    {
        byte[] GetSerializedBytes(MailboxMetricPayload payload);
        MailboxMetricPayload GetObject(byte[] bytes);
    }
}