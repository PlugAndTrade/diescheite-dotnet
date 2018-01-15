using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PlugAndTrade.DieScheite.Client.Common;

namespace PlugAndTrade.DieScheite.Client.AspNetCore
{
    public class Middleware
    {
        private readonly RequestDelegate _next;
        private readonly LogEntryFactory _factory;

        public Middleware(RequestDelegate next, LogEntryFactory factory)
        {
            _next = next;
            _factory = factory;
        }

        public async Task Invoke(HttpContext context, LogEntry entry, IEnumerable<ILogger> loggers)
        {
            entry.Id = context.TraceIdentifier;
            entry.CorrelationId = context.Request.Headers["x-correlation-id"].FirstOrDefault() ?? Guid.NewGuid().ToString();
            entry.ParentId = context.Request.Headers["x-parent-scope-id"].FirstOrDefault();
            try
            {
                await _next(context);
            }
            catch (Exception e)
            {
                entry.Error($"Uncaught exception: {e.Message}", e.StackTrace);
                throw e;
            }
            finally
            {
                entry.Finalize();
                foreach (var logger in loggers)
                {
                    logger.Publish(entry);
                }
            }
        }
    }
}

