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

        public string ServiceFullName { get => $"{_serviceId}-{_instanceId}<{_version}>"; }

        public LogEntry Init(string correlationId, string parentId) =>
            Init(correlationId, parentId, Guid.NewGuid().ToString());

        public LogEntry Init(string correlationId, string parentId, string id)
        {
            return new LogEntry
            {
                Id = id,
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
