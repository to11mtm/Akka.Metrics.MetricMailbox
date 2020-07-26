using Akka.Metrics.MetricMailbox.RedisPubSubListener;
using MessagePack;

namespace WebApplication
{
    public class MessagePackMetricSerializer : IMetricSerializer
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