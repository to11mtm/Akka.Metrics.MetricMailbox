using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Metrics.MetricMailbox.RedisPubSubListener;
using Lib.AspNetCore.ServerSentEvents;
using StackExchange.Redis;

namespace WebApplication
{
    public class SSERedisMetricSubscriber: BaseMetricSubscriber
    {
        private IMailboxEventService _mailboxService;

        public SSERedisMetricSubscriber(IMailboxEventService mailboxService, IConnectionMultiplexer connectionMultiplexer, IMetricSerializer serializer, string keyName) : base(connectionMultiplexer, serializer, keyName)
        {
            _mailboxService = mailboxService;
        }

        public override async Task HandlePublishedMetricsAsync(
            MailboxMetricPayload payload)
        {
            foreach (var metric in payload.Set)
            {
                await _mailboxService.SendEventAsync(new ServerSentEvent()
                {
                    Data = new List<string>()
                    {
                        @"{ ""sender"": """ + metric.Sender + @""",""receiver"": """ + metric.Receiver + @""",""receiverMailBoxSize"": " + metric.ReceiverMailBoxSize + @", ""meassureTimeMillies"": " + metric.SystemTimeMillis + @"}"
                    },
                    Type = "vmm"
                });
                /*await _mailboxService.SendEventAsync(@"{
                          ""sender"": """ + metric.Sender + @""",
                         ""receiver"": """ + metric.Receiver + @""",
                          ""receiverMailBoxSize"": " +
                                               metric.ReceiverMailBoxSize + @"
                          ""meassureTimeMillies"": " +
                                               metric.SystemTimeMillis + @"
                        }");*/
            }

        }
    }
}