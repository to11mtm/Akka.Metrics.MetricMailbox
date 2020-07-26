# Akka.Metrics.MetricMailbox
A Metrics Measuring Mailbox For Akka.NET, inspired by / ported from the JVM Akka's https://github.com/ouven/akka-visualmailbox

### About
The `MetricMeasuringMailbox` is a wrapper around an `UnboundedMailbox` that will send `MailboxMetric` events to the EventStream for every message sent to an actor using the mailbox.

## Usage

Please refer to the GlutenFree.Akka.Metrics.MetricMailbox.VisualizerExample project for a full example with a Visualizer.

Note that unlike the original Scala project, this has been geared to be more 'pluggable' in how metrics are consumed. Rather than assume usage of Akka.IO, an implementation of `IMetricMeasuringWriter` is used so that you may publish metrics in whatever way suits your needs.

#### Notes
 - Currently, The MetricMeasuringMailbox always uses an `UnboundedMailbox` under the hood.
   
 - You cannot currently use this as a Default mailbox. Attempting to do so appears to cause issues in starting the ActorSystem.
 

## Core Library

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
  
#### MetricMeasuringClientListener

The Provided `MetricMeasuringClientListener` is an actor that listens to the event stream and buffers messages, which will be periodically flushed to the `IMetricMeasuringWriter` provided via the constructor.
  - `IMetricMeasuringWriter` has only one Signature, `void WriteMetrics(MailboxMetric[] metric)`
  - If `IMetricMeasuringWriter` fails, the actor will continue to operate. IOW any exceptions thrown when `WriteMetrics()` is called will be caught, logged to the System logger, and not rethrown (i.e. the actor will not restart).

#### MetricsMeasurementPlugin

Simply a nice wrapper for `MetricMeasuringClientListener`


## RedisPubSub

The RedisPubSub Library is designed to provide an `IMetricMeasuringWriter` that publishes to a Redis Pub/Sub channel as well as multiple subscriber abstractions.

 - `MetricPublisher` is an implementation of `IMetricMeasuringWriter`
 - `BaseAsyncMetricSubscriber` provides a `Task HandlePublishedMetricsAsync(MailboxMetricPayload payload)` method to be implemented.
 - `ObservableMetricSubscriber` Wraps the Subscriber and provides an `IObservable<MailboxMetric>` on the `.ObservableSet` property.

In addition to an `IConnectionMultiplexer`, you will also need to provide:

  - An Implementation of `IMetricPayloadSerializer`
  - A `keyName` (string) that should be shared between the Publisher and Subscriber.
   

