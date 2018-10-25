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
        public static IApplicationBuilder UseDieScheite(this IApplicationBuilder app)
        {
            app.UseMiddleware<DieScheiteMiddleware>();
            return app;
        }

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
