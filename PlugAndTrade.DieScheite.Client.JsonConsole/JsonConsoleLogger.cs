using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PlugAndTrade.DieScheite.Client.Common;
using PlugAndTrade.DieScheite.Client.Json;

namespace PlugAndTrade.DieScheite.Client.JsonConsole
{
    public class JsonConsoleLogger : ILogger
    {
        public Task Publish(LogEntry entry) => Task.Run(() =>
        {
            using (var mem = new MemoryStream())
            {
                using (var writer = new StreamWriter(mem))
                {
                    LogEntryJson.WriteJson(writer, entry);
                }
                Console.WriteLine(System.Text.Encoding.UTF8.GetString(mem.ToArray()));
            }

            return Task.CompletedTask;
        });
    }
}
