using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public void LoggedAction(IEnumerable<ILogger> loggers, ITracingScope ts, Action<LogEntry> action)
            => LoggedAction<int>(loggers, ts, (log) =>
            {
                action(log);
                return 0;
            });

        public TResult LoggedAction<TResult>(IEnumerable<ILogger> loggers, ITracingScope ts, Func<LogEntry, TResult> action)
            => LoggedActionAsync(loggers, ts, (log) => Task.FromResult(action(log))).Result;

        public Task LoggedActionAsync(IEnumerable<ILogger> loggers, ITracingScope ts, Func<LogEntry, Task> action)
            => LoggedActionAsync<int>(loggers, ts, (log) => action(log).ContinueWith(x => 0));

        public async Task<TResult> LoggedActionAsync<TResult>(IEnumerable<ILogger> loggers, ITracingScope ts, Func<LogEntry, Task<TResult>> action)
        {
            var entry = Init(ts);
            try
            {
                return await action(entry).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                entry.Error(e.Message, e.StackTrace);
                throw;
            }
            finally
            {
                entry.Finalize();
                foreach (var logger in loggers)
                {
#pragma warning disable 4014
                    logger
                        .Publish(entry)
                        .ContinueWith((t) =>
                        {
                            var e = t.Exception;
                            System.Console.Error.WriteLine($"Error when publishing log entry: {e.Message}");
                            System.Console.Error.WriteLine(e.StackTrace);
                        }, TaskContinuationOptions.OnlyOnFaulted);
#pragma warning restore 4014
                }
            }
        }
    }
}
