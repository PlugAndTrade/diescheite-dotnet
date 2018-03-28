using System;
using Microsoft.Extensions.DependencyInjection;

namespace PlugAndTrade.DieScheite.Client.Common
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDieScheite(
                this IServiceCollection services,
                string serviceId,
                string serviceInstanceId,
                string serviceVersion)
        {
            services.AddSingleton<LogEntryFactory>(sp =>
                new LogEntryFactory(serviceId, serviceInstanceId, serviceVersion)
            );

            services.AddScoped<LogEntry>(sp => sp
                .GetService<LogEntryFactory>()
                .Init(null, null, null)
            );

            return services;
        }
    }
}
