using System;
using System.Linq;
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

        public static LogEntryMessage Log(this LogEntry entry, int level, string message, string stacktrace = null, string traceId = null)
        {
            var msg = new LogEntryMessage
            {
              Level = level,
              Message = message,
              Stacktrace = stacktrace,
              TraceId = traceId
            };
            entry.Messages.Add(msg);
            return msg;
        }

        public static LogEntryMessage Log(this LogEntry entry, LogEntryLevel level, string message, string stacktrace = null, string traceId = null) =>
            entry.Log((int) level, message, stacktrace, traceId);

        public static LogEntryMessage Debug(this LogEntry entry, string message, string traceId = null) =>
            entry.Log((int) LogEntryLevel.Debug, message, null, traceId);

        public static LogEntryMessage Info(this LogEntry entry, string message, string traceId = null) =>
            entry.Log((int) LogEntryLevel.Info, message, null, traceId);

        public static LogEntryMessage Warning(this LogEntry entry, string message, string traceId = null) =>
            entry.Log((int) LogEntryLevel.Warning, message, null, traceId);

        public static LogEntryMessage Error(this LogEntry entry, string message, string stacktrace = null, string traceId = null) =>
            entry.Log((int) LogEntryLevel.Error, message, stacktrace, traceId);

        public static LogEntryMessage Critical(this LogEntry entry, string message, string stacktrace = null, string traceId = null) =>
            entry.Log((int) LogEntryLevel.Critical, message, stacktrace, traceId);

        public static LogEntryMessage Exception(this LogEntry entry, string message, Exception e, LogEntryLevel level = LogEntryLevel.Error, string traceId = null) =>
            entry.Log((int) level, $"{message} ({GetNestedMessages(e)})", GetNestedStackTraces(e), traceId);

        private static string GetNestedMessages(Exception e)
        {
            var msg = $"<{e.GetType().Name}> {e.Message}";
            return e.InnerException == null
                ? msg
                : $"{msg} ({GetNestedMessages(e.InnerException)})";
        }

        private static string GetNestedStackTraces(Exception e) =>
            e.InnerException == null
                ? e.StackTrace
                : $"{e.StackTrace}\n------- INNER STACK TRACE -------\n{GetNestedStackTraces(e.InnerException)}";

        public static LogEntryMessage Attach(this LogEntryMessage message, string name, string contentType, string contentEncoding, byte[] data, ILookup<string, object> headers)
        {
            var attachment = new LogEntryAttachment
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                Headers = headers?.SelectMany(g => g.Select(v => new KeyValuePair<string, object>(g.Key, v))).ToArray() ?? new KeyValuePair<string, object>[0],
                Data = data
            };
            message.Attachments.Add(attachment);
            return message;
        }

        public static LogEntryActiveTrace Trace(this LogEntry entry, string name) =>
            new LogEntryActiveTrace(name, null, (trace) => entry.Trace.Add(trace));

        public static LogEntryActiveTrace Trace(this LogEntry entry, string name, LogEntryActiveTrace parent) =>
            new LogEntryActiveTrace(name, parent, (trace) => entry.Trace.Add(trace));

        public static LogEntry Http(this LogEntry entry, LogEntryHttpData http)
        {
            entry.Http = http;
            entry.Protocol = "http";
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
