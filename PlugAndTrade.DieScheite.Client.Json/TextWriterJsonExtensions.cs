using System;
using System.Collections.Generic;
using System.IO;

namespace PlugAndTrade.DieScheite.Client.Json
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
            writer.WriteJsonPropertyKey(key).WriteJsonPropertyValue(value == null ? (string) null : System.Convert.ToBase64String(value));
        public static TextWriter WriteJsonProperty(this TextWriter writer, string key, bool value) =>
            writer.WriteJsonPropertyKey(key).WriteJsonPropertyValue(value);
        public static TextWriter WriteJsonProperty(this TextWriter writer, string key, int? value) =>
            writer.WriteJsonPropertyKey(key).WriteJsonPropertyValue(value);
        public static TextWriter WriteJsonProperty(this TextWriter writer, string key, int value) =>
            writer.WriteJsonPropertyKey(key).WriteJsonPropertyValue(value);
        public static TextWriter WriteJsonProperty(this TextWriter writer, string key, long value) =>
            writer.WriteJsonPropertyKey(key).WriteJsonPropertyValue(value);
        public static TextWriter WriteJsonProperty(this TextWriter writer, string key, decimal value) =>
            writer.WriteJsonPropertyKey(key).WriteJsonPropertyValue(value);
        public static TextWriter WriteJsonProperty<TValue>(this TextWriter writer, string key, TValue value, Func<TextWriter, TValue, TextWriter> valueWriter) =>
            valueWriter(writer.WriteJsonPropertyKey(key), value);
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

        public static TextWriter WriteJsonPropertyValue(this TextWriter writer, int? value) =>
            value.HasValue ? writer.WriteJsonPropertyValue(value.Value) : writer.WriteJsonPropertyValue((string) null);

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
}
