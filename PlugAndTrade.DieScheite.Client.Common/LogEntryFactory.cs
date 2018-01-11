using System;

namespace PlugAndTrade.DieScheite.Client.Common
{
    public class LogEntryFactory
    {
        private readonly string _serviceId;
        private readonly string _instanceId;
        private readonly string _version;

        public LogEntryFactory(string serviceId, string instanceId, string version)
        {
            _serviceId = serviceId;
            _instanceId = instanceId;
            _version = version;
        }

        public LogEntry Init(string correlationId, string parentId)
        {
            return new LogEntry
            {
                Id = Guid.NewGuid().ToString(),
                ParentId = parentId,
                CorrelationId = correlationId,
                ServiceId = _serviceId,
                ServiceInstanceId = _instanceId,
                ServiceVersion = _version,
                Timestamp = DateTime.UtcNow.AsEpochMillis()
            };
        }

        public void LoggedAction(ILogger logger, string correlationId, string parentId, Action<LogEntry> action)
        {
            var entry = Init(correlationId, parentId);
            try
            {
                action(entry);
            }
            catch (Exception e)
            {
                entry.Error($"Uncaught exception: {e.Message}", e.StackTrace);
            }
            finally
            {
                entry.Finalize();
                logger.Publish(entry);
            }
        }
    }
}
