using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PlugAndTrade.DieScheite.Client.Common;

namespace PlugAndTrade.DieScheite.Client.AspNetCore
{
    public class DieScheiteMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly HashSet<string> _ignoreRoutes;

        public DieScheiteMiddleware(RequestDelegate next, DieScheiteAspNetCoreSettings settings)
        {
            _next = next;
            _ignoreRoutes = new HashSet<string>(settings.IgnoreRoutes);
        }

        public async Task Invoke(HttpContext context, LogEntry entry, IEnumerable<ILogger> loggers)
        {
            if (entry.Id == null)
                entry.Id = Guid.NewGuid().ToString();

            entry.HttpRequest(new LogEntryHttpRequest
            {
                Method = context.Request.Method,
                Uri = $"{context.Request.Path}{context.Request.QueryString.ToString()}",
                Host = context.Request.Host.ToString(),
            });
            entry.HttpResponse(new LogEntryHttpResponse());

            try
            {
                await _next(context);
            }
            catch (Exception e)
            {
                entry.Error($"Uncaught exception: {e.Message}", e.StackTrace);
                throw;
            }
            finally
            {
                entry.Route = context.GetRouteTemplate();

                if (!_ignoreRoutes.Contains(entry.Route))
                {
                    entry.Http.Response.StatusCode = context.Response.StatusCode;

                    entry.Finalize();
                    foreach (var logger in loggers)
                    {
#pragma warning disable 4014
                        logger
                            .Publish(entry)
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
        }
    }
}

