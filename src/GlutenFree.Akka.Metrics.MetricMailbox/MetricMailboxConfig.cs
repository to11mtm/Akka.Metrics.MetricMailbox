using Akka.Configuration;

namespace GlutenFree.Akka.Metrics.MetricMailbox
{
    public static class MetricMailboxConfig
    {
        public static Config Config()
        {
            return ConfigurationFactory.ParseString(
                DefaultMetricMailboxConfigString);
        }
        public static Config WithMetricMailbox(Config config)
        {
            return config.WithFallback(
                Config());
        }

        public static readonly string TypeNameString =
            "GlutenFree.Akka.Metrics.MetricMailbox.MetricMeasuringMailboxType, GlutenFree.Akka.Metrics.MetricMailbox";
        public static readonly string DefaultMetricMailboxConfigString = @"metric-mailbox {
      mailbox-type = """+TypeNameString +@"""
 
      # If the mailbox is bounded then this is the timeout for enqueueing
      # in case the mailbox is full. Negative values signify infinite
      # timeout, which should be avoided as it bears the risk of dead-lock.
        mailbox-push-timeout-time = 10s
 
      # For Actor with Stash: The default capacity of the stash.
      # If negative (or zero) then an unbounded stash is used (default)
      # If positive then a bounded stash is used and the capacity is set using
      # the property
            stash-capacity = -1
    }";
    }
}