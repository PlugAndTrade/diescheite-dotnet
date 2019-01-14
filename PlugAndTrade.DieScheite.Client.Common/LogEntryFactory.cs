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

        public void LoggedAction(IEnumerable<ILogger> loggers, ITracingScope ts, Action<LogEntry> action, bool awaitPublish = false)
            => LoggedAction<int>(loggers, ts, (log) =>
            {
                action(log);
                return 0;
            }, awaitPublish);

        public TResult LoggedAction<TResult>(IEnumerable<ILogger> loggers, ITracingScope ts, Func<LogEntry, TResult> action, bool awaitPublish = false)
            => LoggedActionAsync(loggers, ts, (log) => Task.FromResult(action(log)), awaitPublish).Result;

        public Task LoggedActionAsync(IEnumerable<ILogger> loggers, ITracingScope ts, Func<LogEntry, Task> action, bool awaitPublish = false)
            => LoggedActionAsync<int>(loggers, ts, async (log) =>
            {
                await action(log).ConfigureAwait(false);
                return 0;
            }, awaitPublish);

        public async Task<TResult> LoggedActionAsync<TResult>(IEnumerable<ILogger> loggers, ITracingScope ts, Func<LogEntry, Task<TResult>> action, bool awaitPublish = false)
        {
            return await LoggedActionAsync(loggers, ts, action, t =>
            {
                return awaitPublish ? t : Task.CompletedTask;
            }).ConfigureAwait(false);
        }

        private async Task<TResult> LoggedActionAsync<TResult>(IEnumerable<ILogger> loggers, ITracingScope ts, Func<LogEntry, Task<TResult>> action, Func<Task, Task> onPublish)
        {
            var entry = Init(ts);
            try
            {
                return await action(entry).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                entry.Error($"<{e.GetType().Name}> {e.Message}", e.StackTrace);
                throw;
            }
            finally
            {
                entry.Finalize();
                foreach (var logger in loggers)
                {
#pragma warning disable 4014
                    var pubTask = logger.Publish(entry);
                    pubTask
                        .ContinueWith((t) =>
                        {
                            var e = t.Exception;
                            System.Console.Error.WriteLine($"Error when publishing log entry: {e.Message}");
                            System.Console.Error.WriteLine(e.StackTrace);
                        }, TaskContinuationOptions.OnlyOnFaulted);
                    await onPublish(pubTask).ConfigureAwait(false);
#pragma warning restore 4014
                }
            }
        }
    }
}
