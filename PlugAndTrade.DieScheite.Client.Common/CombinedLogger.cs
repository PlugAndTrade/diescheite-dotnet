using System.Collections.Generic;

namespace PlugAndTrade.DieScheite.Client.Common
{
    public class CombinedLogger : ILogger
    {
        private readonly IReadOnlyCollection<ILogger> _loggers;

        public CombinedLogger(IReadOnlyCollection<ILogger> loggers)
        {
            _loggers = loggers;
        }

        public void Publish(LogEntry entry)
        {
            foreach (var logger in _loggers)
            {
                logger.Publish(entry);
            }
        }
    }
}
