using System;
using PlugAndTrade.TracingScope;

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

        public LogEntry Init(ITracingScope ts)
        {
            return new LogEntry
            {
                Id = ts.ScopeId,
                ParentId = ts.ParentScopeId,
                CorrelationId = ts.CorrelationId,
                ServiceId = _serviceId,
                ServiceInstanceId = _instanceId,
                ServiceVersion = _version,
                Timestamp = DateTime.UtcNow.AsEpochMillis()
            };
        }

        public void LoggedAction(ILogger logger, ITracingScope ts, Action<LogEntry> action)
        {
            var entry = Init(ts);
            try
            {
                action(entry);
            }
            catch (Exception e)
            {
                entry.Error(e.Message, e.StackTrace);
            }
            finally
            {
                entry.Finalize();
                logger.Publish(entry);
            }
        }
    }
}
