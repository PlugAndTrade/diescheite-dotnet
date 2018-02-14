using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using PlugAndTrade.DieScheite.Client.Common;
using PlugAndTrade.RabbitMQ;

namespace PlugAndTrade.DieScheite.Client.RabbitMQ
{
    public static class TextWriterJsonExtensions
    {
        public static TextWriter WriteJsonStartObject(this TextWriter writer)
        {
            writer.Write('{');
            return writer;
        }

        public static TextWriter WriteJsonEndObject(this TextWriter writer)
        {
            writer.Write('}');
            return writer;
        }

        public static TextWriter WriteJsonComma(this TextWriter writer)
        {
            writer.Write(',');
            return writer;
        }

        public static TextWriter WriteJsonProperty(this TextWriter writer, string key, string value) =>
            writer.WriteJsonPropertyKey(key).WriteJsonPropertyValue(value);
        public static TextWriter WriteJsonProperty(this TextWriter writer, string key, byte[] value) =>
            writer.WriteJsonPropertyKey(key).WriteJsonPropertyValue(System.Convert.ToBase64String(value));
        public static TextWriter WriteJsonProperty(this TextWriter writer, string key, bool value) =>
            writer.WriteJsonPropertyKey(key).WriteJsonPropertyValue(value);
        public static TextWriter WriteJsonProperty(this TextWriter writer, string key, int value) =>
            writer.WriteJsonPropertyKey(key).WriteJsonPropertyValue(value);
        public static TextWriter WriteJsonProperty(this TextWriter writer, string key, long value) =>
            writer.WriteJsonPropertyKey(key).WriteJsonPropertyValue(value);
        public static TextWriter WriteJsonProperty(this TextWriter writer, string key, decimal value) =>
            writer.WriteJsonPropertyKey(key).WriteJsonPropertyValue(value);
        public static TextWriter WriteJsonProperty<TValue>(this TextWriter writer, string key, IEnumerable<TValue> values, Func<TextWriter, TValue, TextWriter> valueWriter) =>
            writer.WriteJsonPropertyKey(key).WriteJsonPropertyValue(values, valueWriter);

        public static TextWriter WriteJsonPropertyKey(this TextWriter writer, string key)
        {
            writer.Write('\"');
            writer.Write(key);
            writer.Write("\":");
            return writer;
        }

        public static TextWriter WriteJsonPropertyValue(this TextWriter writer, string value)
        {
            if (value == null)
            {
                writer.Write("null");
            }
            else
            {
                writer.Write('\"');
                foreach (var c in value)
                {
                    switch (c)
                    {
                        case '\r': case '\b': case '\f': break;
                        case '\\': writer.Write(@"\\"); break;
                        case '\t': writer.Write(@"\t"); break;
                        case '\n': writer.Write(@"\n"); break;
                        case '"': writer.Write("\\\""); break;
                        default: writer.Write(c); break;
                    }
                }
                writer.Write('\"');
            }
            return writer;
        }

        public static TextWriter WriteJsonPropertyValue(this TextWriter writer, bool value)
        {
            if (value) writer.Write("true");
            else writer.Write("false");
            return writer;
        }

        public static TextWriter WriteJsonPropertyValue(this TextWriter writer, int value)
        {
            writer.Write(value);
            return writer;
        }

        public static TextWriter WriteJsonPropertyValue(this TextWriter writer, long value)
        {
            writer.Write(value);
            return writer;
        }

        public static TextWriter WriteJsonPropertyValue(this TextWriter writer, double value)
        {
            writer.Write(value.ToString(System.Globalization.CultureInfo.InvariantCulture));
            return writer;
        }

        public static TextWriter WriteJsonPropertyValue(this TextWriter writer, decimal value)
        {
            writer.Write(value.ToString(System.Globalization.CultureInfo.InvariantCulture));
            return writer;
        }

        public static TextWriter WriteJsonPropertyValue<TValue>(this TextWriter writer, IEnumerable<TValue> values, Func<TextWriter, TValue, TextWriter> valueWriter)
        {
            writer.Write('[');
            bool first = true;
            foreach (var value in values)
            {
                if (!first)
                {
                    writer.WriteJsonComma();
                }
                valueWriter(writer, value);
                first = false;
            }
            writer.Write(']');
            return writer;
        }
    }

    public class RabbitMQLogger : ILogger
    {
        private readonly IMessageProducer _producer;

        public RabbitMQLogger(IMessageProducer producer)
        {
            _producer = producer;
        }

        public void Publish(LogEntry entry)
        {
            _producer.Publish("application/json", "gzip", Serialize(entry), "", (props) =>
            {
                if (entry.Messages.Any())
                {
                    props.Headers.Add("Level", entry.Messages.Max(m => m.Level));
                }
                props.Headers.Add("ServiceId", entry.ServiceId);
                foreach (var pair in entry.Headers)
                {
                    props.Headers.Add(pair.Key, pair.Value);
                }
            });
        }

        public static byte[] Serialize(LogEntry entry)
        {
            var mem = new MemoryStream();
            using (var gzip = new GZipStream(mem, CompressionMode.Compress))
            using (var writer = new StreamWriter(gzip))
            {
                RabbitMQLogger.WriteJson(writer, entry);
            }
            return mem.ToArray();
        }

        public static TextWriter WriteJson(TextWriter writer, LogEntry entry)
        {
            return writer
                .WriteJsonStartObject()
                .WriteJsonProperty("id", entry.Id).WriteJsonComma()
                .WriteJsonProperty("parentId", entry.ParentId).WriteJsonComma()
                .WriteJsonProperty("correlationId", entry.CorrelationId).WriteJsonComma()
                .WriteJsonProperty("serviceId", entry.ServiceId).WriteJsonComma()
                .WriteJsonProperty("serviceInstanceId", entry.ServiceInstanceId).WriteJsonComma()
                .WriteJsonProperty("serviceVersion", entry.ServiceVersion).WriteJsonComma()
                .WriteJsonProperty("timestamp", entry.Timestamp).WriteJsonComma()
                .WriteJsonProperty("duration", entry.Duration).WriteJsonComma()
                .WriteJsonProperty("headers", entry.Headers, WriteJson).WriteJsonComma()
                .WriteJsonProperty("messages", entry.Messages, WriteJson).WriteJsonComma()
                .WriteJsonProperty("trace", entry.Trace, WriteJson)
                .WriteJsonEndObject();
        }

        public static TextWriter WriteJson(TextWriter writer, KeyValuePair<string, object> header)
        {
            writer
                .WriteJsonStartObject()
                .WriteJsonPropertyKey(header.Key);
            if (header.Value is string s) writer.WriteJsonPropertyValue(s ?? "");
            else if (header.Value is int i) writer.WriteJsonPropertyValue(i);
            else if (header.Value is long l) writer.WriteJsonPropertyValue(l);
            else if (header.Value is double db) writer.WriteJsonPropertyValue(db);
            else if (header.Value is decimal d) writer.WriteJsonPropertyValue(d);
            else if (header.Value is bool b) writer.WriteJsonPropertyValue(b);
            else writer.WriteJsonPropertyValue("");
            return writer.WriteJsonEndObject();
        }

        public static TextWriter WriteJson(TextWriter writer, LogEntryMessage message)
        {
            return writer
                .WriteJsonStartObject()
                .WriteJsonProperty("level", message.Level).WriteJsonComma()
                .WriteJsonProperty("message", message.Message).WriteJsonComma()
                .WriteJsonProperty("stacktrace", message.Stacktrace).WriteJsonComma()
                .WriteJsonProperty("attachments", message.Attachments, WriteJson)
                .WriteJsonEndObject();
        }

        public static TextWriter WriteJson(TextWriter writer, LogEntryAttachment attachment)
        {
            return writer
                .WriteJsonStartObject()
                .WriteJsonProperty("id", attachment.Id).WriteJsonComma()
                .WriteJsonProperty("name", attachment.Name).WriteJsonComma()
                .WriteJsonProperty("contentType", attachment.ContentType).WriteJsonComma()
                .WriteJsonProperty("contentEncoding", attachment.ContentEncoding).WriteJsonComma()
                .WriteJsonProperty("data", attachment.Data)
                .WriteJsonEndObject();
        }

        public static TextWriter WriteJson(TextWriter writer, LogEntryTrace trace)
        {
            return writer
                .WriteJsonStartObject()
                .WriteJsonProperty("id", trace.Id).WriteJsonComma()
                .WriteJsonProperty("parentId", trace.ParentId).WriteJsonComma()
                .WriteJsonProperty("name", trace.Name).WriteJsonComma()
                .WriteJsonProperty("timestamp", trace.Timestamp).WriteJsonComma()
                .WriteJsonProperty("duration", trace.Duration)
                .WriteJsonEndObject();
        }
    }
}
