using System;
using System.Threading;
using Akka.Actor;
using Akka.Configuration;
using Akka.Mailbox.Visualizer;
using Xunit;

namespace Akka.Metrics.MetricMailbox.Tests
{
    public class MetricsMeasuringMailboxTests
    {
        
        [Fact]
        public void MetricsMailbox_Records_Metrics()
        {
            
            var testSystem = ActorSystem.Create("test",
                MetricMailboxConfig.Config());
            
            var metricsMock = new MetricsMock();
            testSystem.RegisterMailboxMetricListener(metricsMock);
            var testActor =
                testSystem.ActorOf(Props.Create(() => new EchoActor()).WithDefaultMetricMailbox());
            testActor.Tell(new Echo());
            SpinWait.SpinUntil(() => false, TimeSpan.FromSeconds(3));
            Assert.True(metricsMock.messagesReceived > 0);
        }
    }

    public class EchoActor : ReceiveActor
    {
        public EchoActor()
        {
            Receive<Echo>(echo => Context.Sender.Tell(echo));
        }
    }

    public class Echo
    {
        
    }

    public class MetricsMock : IMetricMeasuringWriter
    {
        public int messagesReceived;
        public void WriteMetrics(MailboxMetric[] metric)
        {
            messagesReceived = messagesReceived + metric.Length;
        }
    }
}