using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlugAndTrade.DieScheite.Client.Common;

namespace PlugAndTrade.DieScheite.Client.Console
{
    public class ConsoleLogger : ILogger
    {
        private readonly IReadOnlyCollection<Func<LogEntry, bool>> _filters;

        public ConsoleLogger()
        {
            _filters = new Func<LogEntry, bool>[0];
        }

        public ConsoleLogger(IReadOnlyCollection<Func<LogEntry, bool>> filters)
        {
            _filters = filters.ToArray();
        }

        public ConsoleLogger(LogEntryLevel minLevel)
        {
            _filters = new Func<LogEntry, bool>[]
            {
                e => e.Level >= (int) minLevel
            };
        }

        public Task Publish(LogEntry entry) => Task.Run(() =>
        {
            if (_filters.Any() && _filters.All(f => !f(entry)))
            {
                return Task.CompletedTask;
            }

            var time = DateTimeOffset
                .FromUnixTimeMilliseconds(entry.Timestamp)
                .DateTime
                .ToLocalTime();
            System.Console.WriteLine($"{time:yyyy-MM-dd HH:mm:ss.fff} [{entry.Duration}ms]: {entry.ServiceId}<{entry.ServiceVersion}>({entry.ServiceInstanceId}) {{");

            if (entry.Http != null)
            {
                System.Console.WriteLine($"{entry.Http.Request.Method} {entry.Http.Request.Uri} <- {entry.Http.Response.StatusCode}");
            }

            foreach (var message in entry.Messages)
            {
                System.Console.WriteLine($" * [{message.Level}]<{message.TraceId}>: {message.Message}");
                if (message.Stacktrace != null)
                {
                    System.Console.WriteLine(message.Stacktrace);
                }
            }

            if (entry.Trace.Count > 0)
            {
                System.Console.WriteLine("Traces:");
                var traces = entry.Trace.ToLookup(t => t.ParentId ?? "");
                foreach (var trace in traces[""]) PrintTrace(trace, traces);
            }

            System.Console.WriteLine("}");
            return Task.CompletedTask;
        });

        private void PrintTrace(LogEntryTrace trace, ILookup<string, LogEntryTrace> traces, int depth = 1)
        {
            System.Console.WriteLine($" {string.Join("", Enumerable.Repeat(">", depth))} {trace.Name}: {trace.Duration}ms {trace.Timestamp}");
            foreach (var childTrace in traces[trace.Id]) PrintTrace(childTrace, traces, depth + 1);
        }
    }
}
