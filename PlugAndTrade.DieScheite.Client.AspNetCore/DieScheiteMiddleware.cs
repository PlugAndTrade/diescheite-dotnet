using System;
using System.IO;
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

        public DieScheiteMiddleware(RequestDelegate next)
        {
            _next = next;
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

            var requestBody = await GetBody(context.Request);
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
                entry.HttpResponse(new LogEntryHttpResponse
                {
                    StatusCode = context.Response.StatusCode
                });

                if (context.Response.StatusCode >= 400)
                {
                    entry.Http.Request.Body = requestBody.Length > 0 ? requestBody : null;
                    foreach (var header in context.Request.Headers)
                    {
                        entry.Http.Request.Headers.Add(new KeyValuePair<string, object>(header.Key, header.Value.First()));
                    }
                    foreach (var header in context.Response.Headers)
                    {
                        entry.Http.Response.Headers.Add(new KeyValuePair<string, object>(header.Key, header.Value.First()));
                    }
                }

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

        private async Task<byte[]> GetBody(HttpRequest request)
        {
            var mem = new MemoryStream();
            await request.Body.CopyToAsync(mem);
            var res = mem.ToArray();
            if (!request.Body.CanSeek)
            {
                mem.Seek(0, SeekOrigin.Begin);
                request.Body = mem;
            }
            else
            {
                request.Body.Seek(0, SeekOrigin.Begin);
            }
            return res;
        }
    }
}

