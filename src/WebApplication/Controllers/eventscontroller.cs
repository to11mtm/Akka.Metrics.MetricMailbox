using System;
using System.Threading;
using System.Threading.Tasks;
using Akka.Mailbox.Visualizer;
using Akka.Metrics.MetricMailbox.RedisPubSubListener;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers
{
    /*[Route("/api/observableevents")]
    public class ObservableEventsController : Controller
    {
        private IObservable<MailboxMetric> _metricObservable; 
        public ObservableEventsController(
            ObservableMetricSubscriber subscriber)
        {
            _metricObservable = subscriber.ObservableSet;
        }
        
        [HttpGet]
        public async Task Get(CancellationToken cancellationToken)
        {
            var response = Response;
            response.Headers.Add("Content-Type", "text/event-stream");
            await response.Body.FlushAsync(cancellationToken);
            using (var subscription = _metricObservable.SubscribeAsync(
                async metric =>
                {
                    await response.WriteAsync(@"{
                          ""sender"": """ + metric.Sender + @""",
                         ""receiver"": """ + metric.Receiver + @""",
                          ""receiverMailBoxSize"": " +
                                              metric.ReceiverMailBoxSize + @"
                          ""meassureTimeMillies"": " +
                                              metric.SystemTimeMillis + @"
                        }",cancellationToken);
                    await response.Body.FlushAsync(cancellationToken);
                }))
            {
                while (!cancellationToken.IsCancellationRequested) {
                    await Task.Delay(500,cancellationToken);
                }
                GC.KeepAlive(subscription);
            }
            
                
            // ReSharper disable once FunctionNeverReturns
        }
    }
*/
    
}