using System.Collections.Generic;
using System.Threading.Tasks;
using GlutenFree.Akka.Metrics.MetricMailbox.RedisPubSubListener;
using Lib.AspNetCore.ServerSentEvents;
using StackExchange.Redis;

namespace GlutenFree.Akka.Metrics.MetricMailbox.VisualizerExample
{
    public class SseRedisAsyncMetricSubscriber: BaseAsyncMetricSubscriber
    {
        private IMailboxEventService _mailboxService;

        public SseRedisAsyncMetricSubscriber(IMailboxEventService mailboxService, IConnectionMultiplexer connectionMultiplexer, IMetricPayloadSerializer payloadSerializer, string keyName) : base(connectionMultiplexer, payloadSerializer, keyName)
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