using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlugAndTrade.DieScheite.Client.Common;
using PlugAndTrade.RabbitMQ;
using PlugAndTrade.TracingScope;

namespace PlugAndTrade.DieScheite.Client.RabbitMQ
{
    public class DieScheiteRabbitMQMiddleware
    {
        private readonly LogEntryFactory _logEntryFactory;
        private readonly IEnumerable<ILogger> _loggers;

        public DieScheiteRabbitMQMiddleware(LogEntryFactory logEntryFactory, IEnumerable<ILogger> loggers)
        {
            _logEntryFactory = logEntryFactory;
            _loggers = loggers;
        }

        public async Task<bool> Invoke(Message message, Func<Tuple<Message, LogEntry>, Task<bool>> next)
        {
            var logEntry = _logEntryFactory.Init(new StaticTracingScope
            {
                ScopeId = Guid.NewGuid().ToString(),
                CorrelationId = message.CorrelationId ?? Guid.NewGuid().ToString(),
                ParentScopeId = GetHeader(message, "X-Parent-Scope-Id")
            });

            logEntry.AddHeader("RabbitMQMessageId", message.MessageId);

            try
            {
                return await next(Tuple.Create(message, logEntry));
            }
            catch (Exception e)
            {
                logEntry.Error($"Uncaught exception: {e.Message}", e.StackTrace);
                throw;
            }
            finally
            {
                logEntry.Finalize();
                foreach (var logger in _loggers)
                {
#pragma warning disable 4014
                    logger
                        .Publish(logEntry)
                        .ContinueWith((t) =>
                        {
                            var e = t.Exception;
                            System.Console.Error.WriteLine($"Error when publishing log entry: {e.Message}");
                            System.Console.Error.WriteLine(e.StackTrace);
                        }, TaskContinuationOptions.OnlyOnFaulted);
#pragma warning restore 4014
                }
            }
        }

        public static Func<Tuple<T, LogEntry>, Func<Tuple<TNext, LogEntry>, Task<bool>>, Task<bool>> WrapLogEntry<T, TNext>(Func<T, Func<TNext, Task<bool>>, Task<bool>> middleware) =>
            (message, next) => middleware(message.Item1, (TNext nextMessage) => next(Tuple.Create(nextMessage, message.Item2)));

        private static string GetHeader(Message msg, string key, string def = null)
        {
            object val;
            return msg.Headers.TryGetValue(key, out val) ? val as string: def;
        }
    }
}
