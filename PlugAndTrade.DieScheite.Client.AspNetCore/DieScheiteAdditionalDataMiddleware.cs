using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PlugAndTrade.DieScheite.Client.Common;

namespace PlugAndTrade.DieScheite.Client.AspNetCore
{
    public class DieScheiteAdditionalDataMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly Func<HttpContext, bool> _shouldLogRequestBody;
        private readonly HashSet<string> _censoredHeaders;

        public DieScheiteAdditionalDataMiddleware(
            RequestDelegate next,
            DieScheiteAspNetCoreSettings settings)
        {
            _next = next;
            _shouldLogRequestBody = settings.ShouldLogRequestBody;
            _censoredHeaders = new HashSet<string>(settings.CensoredHeaders);
        }

        public async Task Invoke(HttpContext context, LogEntry entry)
        {
            var requestBody = await GetBody(context.Request);

            foreach (var header in context.Request.Headers)
            {
                entry.Http.Request.Headers.Add(new KeyValuePair<string, object>(
                    header.Key,
                    _censoredHeaders.Contains(header.Key) ? "<censored>" : header.Value.First()
                ));
            }

            await _next(context);

            foreach (var header in context.Response.Headers)
            {
                entry.Http.Response.Headers.Add(new KeyValuePair<string, object>(
                    header.Key,
                    _censoredHeaders.Contains(header.Key) ? "<censored>" : header.Value.First()
                ));
            }

            if (_shouldLogRequestBody(context))
            {
                entry.Http.Request.Body = requestBody.Length > 0 ? requestBody : null;
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
