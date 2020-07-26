using Lib.AspNetCore.ServerSentEvents;
using Microsoft.Extensions.Options;

namespace GlutenFree.Akka.Metrics.MetricMailbox.VisualizerExample
{
    public class MailboxEventService : ServerSentEventsService ,IMailboxEventService
    {
        public MailboxEventService(IOptions<ServerSentEventsServiceOptions<ServerSentEventsService>> options) : base(options)
        {
        }
    }
}