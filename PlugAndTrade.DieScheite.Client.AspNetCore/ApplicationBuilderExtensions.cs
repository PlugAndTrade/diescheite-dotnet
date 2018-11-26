using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using PlugAndTrade.DieScheite.Client.Common;

namespace PlugAndTrade.DieScheite.Client.AspNetCore
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseDieScheite(this IApplicationBuilder app, DieScheiteAspNetCoreSettings settings)
        {
            var s = DieScheiteAspNetCoreSettings.Default(settings);
            return app
                .UseMiddleware<DieScheiteMiddleware>(s)
                .UseMiddleware<DieScheiteAdditionalDataMiddleware>(s);
        }

        public static IApplicationBuilder UseDieScheite(this IApplicationBuilder app, Func<HttpContext, bool> shouldLogRequestBody, string[] censoredHeaders) =>
            UseDieScheite(app, new DieScheiteAspNetCoreSettings { CensoredHeaders = censoredHeaders, ShouldLogRequestBody = shouldLogRequestBody });

        public static IApplicationBuilder UseDieScheite(this IApplicationBuilder app, string[] censoredHeaders) =>
            UseDieScheite(app, new DieScheiteAspNetCoreSettings { CensoredHeaders = censoredHeaders });

        public static IApplicationBuilder UseDieScheite(this IApplicationBuilder app, Func<HttpContext, bool> shouldLogRequestBody) =>
            UseDieScheite(app, new DieScheiteAspNetCoreSettings { ShouldLogRequestBody = shouldLogRequestBody });

        public static IApplicationBuilder UseDieScheite(this IApplicationBuilder app) =>
            UseDieScheite(app, new DieScheiteAspNetCoreSettings());

        public static IMvcBuilder AddDieScheiteAspNetCore(this IMvcBuilder mvcBuilder)
        {
            mvcBuilder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<MvcOptions>, DieScheiteMvcOptionsSetup>());
            return mvcBuilder;
        }

        public static IMvcCoreBuilder AddDieScheiteAspNetCore(this IMvcCoreBuilder mvcBuilder)
        {
            mvcBuilder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<MvcOptions>, DieScheiteMvcOptionsSetup>());
            return mvcBuilder;
        }

        private class DieScheiteMvcOptionsSetup : IConfigureOptions<MvcOptions>
        {
            public void Configure(MvcOptions options)
            {
                if (!options.Filters.OfType<DieScheiteResourceFilter>().Any())
                {
                    options.Filters.Add(new DieScheiteResourceFilter(new MvcRouteTemplateResolver()));
                }
            }
        }
    }
}
