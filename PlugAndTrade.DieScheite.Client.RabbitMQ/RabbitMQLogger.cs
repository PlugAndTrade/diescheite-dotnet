using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlugAndTrade.DieScheite.Client.Common;
using PlugAndTrade.DieScheite.Client.Json;
using PlugAndTrade.RabbitMQ;

namespace PlugAndTrade.DieScheite.Client.RabbitMQ
{
    public class RabbitMQLogger : ILogger
    {
        private readonly IMessageProducer _producer;

        public RabbitMQLogger(IMessageProducer producer)
        {
            _producer = producer;
        }

        public Task Publish(LogEntry entry) => Task.Run(() =>
        {
            _producer.Publish("application/json", "gzip", Serialize(entry), "", (props) =>
            {
                props.Headers.Add("Level", entry.Level);
                props.Headers.Add("ServiceId", entry.ServiceId);
                foreach (var pair in entry.Headers)
                {
                    props.Headers.Add(pair.Key, pair.Value);
                }
            });
        });

        public static byte[] Serialize(LogEntry entry)
        {
            var mem = new MemoryStream();
            using (var gzip = new GZipStream(mem, CompressionMode.Compress))
            using (var writer = new StreamWriter(gzip))
            {
                LogEntryJson.WriteJson(writer, entry);
            }
            return mem.ToArray();
        }
    }
}
