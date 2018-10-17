using System.Collections.Generic;

namespace PlugAndTrade.DieScheite.Client.Common
{
    public class LogEntryRabbitMQData
    {
        public string QueueName { get; set; }
        public bool Acked { get; set; }
        public string MessageId { get; set; }
    }
}
