using System;
using Microsoft.AspNetCore.Http;

namespace PlugAndTrade.DieScheite.Client.AspNetCore
{
    public class DieScheiteAspNetCoreSettings
    {
        public static Func<HttpContext, bool> LogRequestOnServerError = context => context.Response.StatusCode >= 500;
        public static string[] DefaultCensoredHeaders = new [] { "Authorization", "User-Agent" };
        public static string[] DefaultIgnoreRoutes = new [] { "healthcheck" };

        public string[] IgnoreRoutes { get; set; }
        public string[] CensoredHeaders { get; set; }
        public Func<HttpContext, bool> ShouldLogRequestBody { get; set; }

        public static DieScheiteAspNetCoreSettings Default(DieScheiteAspNetCoreSettings s) => new DieScheiteAspNetCoreSettings
        {
            IgnoreRoutes = s?.IgnoreRoutes ?? DefaultIgnoreRoutes,
            CensoredHeaders = s?.CensoredHeaders ?? DefaultCensoredHeaders,
            ShouldLogRequestBody = s?.ShouldLogRequestBody ?? LogRequestOnServerError
        };
    }
}
