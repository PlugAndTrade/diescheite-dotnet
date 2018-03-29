using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PlugAndTrade.DieScheite.Client.Common
{
    public static class LogEntryExtensions
    {
        public static long AsEpochMillis(this DateTime d)
          => (long) (d - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;

        public static LogEntry Finalize(this LogEntry entry)
        {
            entry.Duration = DateTime.UtcNow.AsEpochMillis() - entry.Timestamp; 
            return entry;
        }

        private static LogEntry AddHeader(this LogEntry entry, string key, object value)
        {
            entry.Headers.Add(new KeyValuePair<string, object>(key, value));
            return entry;
        }

        public static LogEntry AddHeader(this LogEntry entry, string key, string value) =>
            value != null ? entry.AddHeader(key, (object) value) : entry;
        public static LogEntry AddHeader(this LogEntry entry, string key, int value) =>
            entry.AddHeader(key, (object) value);
        public static LogEntry AddHeader(this LogEntry entry, string key, long value) =>
            entry.AddHeader(key, (object) value);
        public static LogEntry AddHeader(this LogEntry entry, string key, double value) =>
            entry.AddHeader(key, (object) value);
        //public static LogEntry AddHeader(this LogEntry entry, string key, decimal value) =>
            //entry.AddHeader(key, (object) value);
        public static LogEntry AddHeader(this LogEntry entry, string key, bool value) =>
            entry.AddHeader(key, (object) value);

        public static LogEntry Log(this LogEntry entry, int level, string message, string stacktrace = null, string traceId = null)
        {
            entry.Messages.Add(new LogEntryMessage
            {
              Level = level,
              Message = message,
              Stacktrace = stacktrace,
              TraceId = traceId
            });
            return entry;
        }
        public static LogEntry Log(this LogEntry entry, LogEntryLevel level, string message, string stacktrace = null, string traceId = null) =>
            entry.Log((int) level, message, stacktrace, traceId);

        public static LogEntry Debug(this LogEntry entry, string message, string traceId = null) =>
            entry.Log((int) LogEntryLevel.Debug, message, null, traceId);
        public static LogEntry Info(this LogEntry entry, string message, string traceId = null) =>
            entry.Log((int) LogEntryLevel.Info, message, null, traceId);
        public static LogEntry Warning(this LogEntry entry, string message, string traceId = null) =>
            entry.Log((int) LogEntryLevel.Warning, message, null, traceId);
        public static LogEntry Error(this LogEntry entry, string message, string stacktrace = null, string traceId = null) =>
            entry.Log((int) LogEntryLevel.Error, message, stacktrace, traceId);
        public static LogEntry Critical(this LogEntry entry, string message, string stacktrace = null, string traceId = null) =>
            entry.Log((int) LogEntryLevel.Critical, message, stacktrace, traceId);

        public static LogEntryActiveTrace Trace(this LogEntry entry, string name) =>
            new LogEntryActiveTrace(name, null, (trace) => entry.Trace.Add(trace));
        public static LogEntryActiveTrace Trace(this LogEntry entry, string name, LogEntryActiveTrace parent) =>
            new LogEntryActiveTrace(name, parent, (trace) => entry.Trace.Add(trace));

        public static LogEntry Http(this LogEntry entry, LogEntryHttpData http)
        {
            entry.Http = http;
            return entry;
        }

        public static LogEntry HttpRequest(this LogEntry entry, LogEntryHttpRequest request)
        {
            if (entry.Http == null)
            {
                return entry.Http(new LogEntryHttpData { Request = request });
            }
            else
            {
                entry.Http.Request = request;
                return entry;
            }
        }

        public static LogEntry HttpResponse(this LogEntry entry, LogEntryHttpResponse response)
        {
            if (entry.Http == null)
            {
                return entry.Http(new LogEntryHttpData { Response = response });
            }
            else
            {
                entry.Http.Response = response;
                return entry;
            }
        }
    }
}
