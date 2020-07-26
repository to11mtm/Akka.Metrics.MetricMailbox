using GlutenFree.Akka.Metrics.MetricMailbox.RedisPubSubListener;
using MessagePack;

namespace GlutenFree.Akka.Metrics.MetricMailbox.VisualizerExample
{
    public class MessagePackMetricPayloadSerializer : IMetricPayloadSerializer
    {
        public byte[] GetSerializedBytes(MailboxMetricPayload payload)
        {
            return MessagePackSerializer.Typeless.Serialize(payload);
        }

        public MailboxMetricPayload GetObject(byte[] bytes)
        {
            return MessagePackSerializer.Typeless.Deserialize(bytes) as
                MailboxMetricPayload;
        }
    }
}