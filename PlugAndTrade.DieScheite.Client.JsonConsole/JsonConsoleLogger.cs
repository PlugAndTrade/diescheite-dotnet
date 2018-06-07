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
        public JsonConsoleLogger()
        {
        }

        public Task Publish(LogEntry entry) => Task.Run(() =>
        {
            LogEntryJson.WriteJson(Console.Out, entry);
            Console.WriteLine();

            return Task.CompletedTask;
        });
    }
}
