using System;
using System.IO;
using System.Linq;
using System.Threading;
using PlugAndTrade.DieScheite.Client.Common;
using PlugAndTrade.DieScheite.Client.Console;
using PlugAndTrade.DieScheite.Client.JsonConsole;
using PlugAndTrade.DieScheite.Client.RabbitMQ;
using PlugAndTrade.RabbitMQ;

namespace PlugAndTrade.DieScheite.Client.Example
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var factory = new LogEntryFactory("PlugAndTrade.DieScheite.Client.Example", "01", "1.0.0");
            var logger = args.Length == 0
                ? (ILogger) new ConsoleLogger(LogEntryLevel.Info)
                : new CombinedLogger(args.Select(a =>
                    a == "rabbitmq" ? (ILogger) new RabbitMQLogger(
                        new RabbitMQClientFactory("localhost", 5672, "die-scheite.client.example")
                        .CreateProducer("diescheite", 1000)
                    )
                    : a == "console" ? (ILogger) new ConsoleLogger(LogEntryLevel.Info)
                    : a == "json" ? (ILogger) new JsonConsoleLogger()
                    : null
                )
                .Where(l => l != null)
                .ToArray());

            while (true)
            {
                factory.LoggedAction(logger, "<correlationId>", null, (entry) =>
                {
                    entry
                        .AddHeader("SomeString", "string")
                        .AddHeader("SomeInt", 1)
                        .AddHeader("SomeLong", 1000000000000L)
                        .AddHeader("SomeDecimal", 1.1)
                        .AddHeader("SomeBoolT", true)
                        .AddHeader("SomeBoolF", false);

                    entry.Info("Begin job");
                    using (var trace = entry.Trace("first-trace"))
                    {
                        entry.Info("Start doing some work...", trace.Id);
                        DoWork();
                        entry.Info("done", trace.Id);
                    }

                    using (var trace = entry.Trace("second-trace"))
                    {
                        entry.Info("Start doing faiing work...", trace.Id);
                        try
                        {
                          DoError();
                        }
                        catch (Exception e)
                        {
                          entry.Error(e.Message, e.StackTrace, trace.Id);
                        }

                        entry.Info("Nested tracing...", trace.Id);
                        using (var nestedTrace = entry.Trace("nested-trace", trace))
                        {
                            entry.Info("in nested trace", nestedTrace.Id);
                        }
                        entry.Info("nested done", trace.Id);

                        entry.Info("done", trace.Id);
                    }
                    entry.Info("End job");
                });

                Thread.Sleep(1000);
            }
        }

        public static int DoWork()
        {
            return 0;
        }

        public static int DoError()
        {
            throw new Exception("Oups...");
        }
    }
}
