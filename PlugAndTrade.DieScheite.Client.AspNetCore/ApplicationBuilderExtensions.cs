using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using PlugAndTrade.DieScheite.Client.Common;

namespace PlugAndTrade.DieScheite.Client.AspNetCore
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseDieScheite(this IApplicationBuilder app)
        {
            app.UseMiddleware<DieScheiteMiddleware>();
            return app;
        }
    }
}
