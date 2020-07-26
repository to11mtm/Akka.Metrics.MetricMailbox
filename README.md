# Akka.Metrics.MetricMailbox
A Metrics Measuring Mailbox For Akka.NET, inspired by / ported from the JVM Akka's https://github.com/ouven/akka-visualmailbox

### About
The `MetricMeasuringMailbox` is a wrapper around an `UnboundedMailbox` that will send `MailboxMetric` events to the EventStream for every message sent to an actor using the mailbox.

## Usage

To use the Metric Mailbox, you will need to do the following:

 - Ensure that a Mailbox Configuration is provided in the ActorSystem Configuration:
   - Default configuration/declaration is provided in the `MetricMailboxConfig` class in both String and `Config` object form.
   - An Extension Method are provided to inject the Default configuration via `config.WithDefaultMetricMailbox()`
 - For Actors that you wish to track metrics for, include the mailbox in your props:
   - Example: `Props.Create(()=>new MyActor()).WithMailbox("metric-mailbox")`
   - An Extension Method is provided to use the Default Metric Mailbox: `Props.Create(()=>new MyActor()).WithDefaultMetricMailbox()`

You may then utilize the Metrics via one of the following options:
  - Create your own actor that subscribes to `MailboxMetric` messages on the EventStream
  - Use the Provided `MetricMeasuringClientListener`
  
### MetricMeasuringClientListener

The Provided `MetricMeasuringClientListener` is an actor that listens to the event stream and buffers messages, which will be periodically flushed to the `IMetricMeasuringWriter` provided via the constructor.
  - `IMetricMeasuringWriter` has only one Signature, `void WriteMetrics(MailboxMetric[] metric)`
  - If `IMetricMeasuringWriter` fails, the actor will continue to operate. IOW any exceptions thrown when `WriteMetrics()` is called will be caught, logged to the System logger, and not rethrown (i.e. the actor will not restart).

### MetricsMeasurementTool

Simply a nice wrapper for `MetricMeasuringClientListener`
  
   

