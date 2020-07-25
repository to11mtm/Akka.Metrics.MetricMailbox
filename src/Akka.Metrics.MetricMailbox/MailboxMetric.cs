namespace Akka.Mailbox.Visualizer
{
    public class MailboxMetric
    {
        public MailboxMetric(string sender, string receiver, int receiverMailBoxSize, long systemTimeMillis)
        {
            Sender = sender;
            Receiver = receiver;
            ReceiverMailBoxSize = receiverMailBoxSize;
            SystemTimeMillis = systemTimeMillis;
        }

        public string Sender { get; }
        public string Receiver { get; }
        public int ReceiverMailBoxSize { get; }
        public long SystemTimeMillis { get; }
    }
}