using System;
using Microsoft.Extensions.DependencyInjection;
using PlugAndTrade.TracingScope;

namespace PlugAndTrade.DieScheite.Client.Common
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDieScheite(
                this IServiceCollection services,
                string serviceId,
                string serviceInstanceId,
                string serviceVersion,
                Func<IServiceProvider, ITracingScopeAccessor> tracingScopeProvider)
        {
            return services
                .AddSingleton<LogEntryFactory>(sp =>
                    new LogEntryFactory(serviceId, serviceInstanceId, serviceVersion)
                )
                .AddScoped<LogEntry>(sp => sp
                    .GetService<LogEntryFactory>()
                    .Init(tracingScopeProvider(sp).CurrentTracingScope)
                )
                ;
        }
    }
}
