using System;
using System.Linq;
using PlugAndTrade.DieScheite.Client.Common;

namespace PlugAndTrade.DieScheite.Client.Console
{
    public class ConsoleLogger : ILogger
    {
        public void Publish(LogEntry entry)
        {
            System.Console.WriteLine($"{entry.Timestamp}[{entry.Duration}ms]: {entry.ServiceId}<{entry.ServiceVersion}>({entry.ServiceInstanceId}) {{");

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

            System.Console.WriteLine("Traces:");
            var traces = entry.Trace.ToLookup(t => t.ParentId ?? "");
            foreach (var trace in traces[""]) PrintTrace(trace, traces);

            System.Console.WriteLine("}");
        }

        private void PrintTrace(LogEntryTrace trace, ILookup<string, LogEntryTrace> traces, int depth = 1)
        {
            System.Console.WriteLine($" {string.Join("", Enumerable.Repeat(">", depth))} {trace.Name}<{trace.Id}>: {trace.Duration}ms {trace.Timestamp}");
            foreach (var childTrace in traces[trace.Id]) PrintTrace(childTrace, traces, ++depth);
        }
    }
}
