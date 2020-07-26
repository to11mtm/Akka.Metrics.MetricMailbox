using Lib.AspNetCore.ServerSentEvents;
using Microsoft.Extensions.Options;

namespace WebApplication
{
    public class MailboxEventService : ServerSentEventsService ,IMailboxEventService
    {
        public MailboxEventService(IOptions<ServerSentEventsServiceOptions<ServerSentEventsService>> options) : base(options)
        {
        }
    }
}