using System.Collections.Generic;
using System.IO;
using System.Linq;
using PlugAndTrade.DieScheite.Client.Common;

namespace PlugAndTrade.DieScheite.Client.Json
{
    public static class LogEntryJson
    {
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
                .WriteJsonProperty("protocol", entry.Protocol).WriteJsonComma()
                .WriteJsonProperty("route", entry.Route).WriteJsonComma()
                .WriteJsonProperty("timestamp", entry.Timestamp).WriteJsonComma()
                .WriteJsonProperty("duration", entry.Duration).WriteJsonComma()
                .WriteJsonProperty("level", entry.Level).WriteJsonComma()
                .WriteJsonProperty("levelCategory", entry.LevelCategory).WriteJsonComma()
                .WriteJsonProperty("headers", entry.Headers, WriteJson).WriteJsonComma()
                .WriteJsonProperty("http", entry.Http, WriteJson).WriteJsonComma()
                .WriteJsonProperty("rabbitmq", entry.RabbitMQ, WriteJson).WriteJsonComma()
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
                .WriteJsonProperty("traceId", message.TraceId).WriteJsonComma()
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
                .WriteJsonProperty("headers", attachment.Headers, WriteJson).WriteJsonComma()
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

        public static TextWriter WriteJson(TextWriter writer, LogEntryHttpData http)
        {
            if (http == null)
            {
                writer.Write("null");
                return writer;
            }

            return writer
            .WriteJsonStartObject()
            .WriteJsonPropertyKey("request")
                .WriteJsonStartObject()
                .WriteJsonProperty("method", http.Request.Method).WriteJsonComma()
                .WriteJsonProperty("uri", http.Request.Uri).WriteJsonComma()
                .WriteJsonProperty("host", http.Request.Host).WriteJsonComma()
                .WriteJsonProperty("headers", http.Request.Headers, WriteJson).WriteJsonComma()
                .WriteJsonProperty("body", http.Request.Body)
                .WriteJsonEndObject().WriteJsonComma()
            .WriteJsonPropertyKey("response")
                .WriteJsonStartObject()
                .WriteJsonProperty("statusCode", http.Response.StatusCode).WriteJsonComma()
                .WriteJsonProperty("headers", http.Response.Headers, WriteJson).WriteJsonComma()
                .WriteJsonProperty("body", http.Response.Body)
                .WriteJsonEndObject()
            .WriteJsonEndObject();
        }

        public static TextWriter WriteJson(TextWriter writer, LogEntryRabbitMQData message)
        {
            if (message == null)
            {
                writer.Write("null");
                return writer;
            }

            return writer
                .WriteJsonStartObject()
                .WriteJsonProperty("messageId", message.MessageId).WriteJsonComma()
                .WriteJsonProperty("queueName", message.QueueName).WriteJsonComma()
                .WriteJsonProperty("acked", message.Acked)
                .WriteJsonEndObject();
        }

    }
}
