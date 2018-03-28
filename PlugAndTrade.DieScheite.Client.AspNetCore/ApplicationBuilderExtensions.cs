using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PlugAndTrade.RabbitMQ;
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

        [Obsolete("Use PlugAndTrade.DieScheite.Client.Console.AddDieScheiteConsole(IServiceCollection) instead.")]
        public static IServiceCollection AddDieScheiteConsole(this IServiceCollection services) =>
            PlugAndTrade.DieScheite.Client.Console.DependencyInjection.AddDieScheiteConsole(services);

        [Obsolete("Use PlugAndTrade.DieScheite.Client.RabbitMQ.AddDieScheiteRabbitMQ(IServiceCollection, Func<IServiceProvider, RabbitMQClientFactory>, string, [int]) instead.")]
        public static IServiceCollection AddDieScheiteRabbitMQ(
                this IServiceCollection services,
                string host,
                int port,
                string exchange,
                int timeout = 60)
        {
            services.AddSingleton<RabbitMQClientFactory>(sp =>
                new RabbitMQClientFactory(host, port, sp.GetService<LogEntryFactory>().ServiceFullName)
            );
            return PlugAndTrade.DieScheite.Client.RabbitMQ.DependencyInjection.AddDieScheiteRabbitMQ(
                services,
                (sp) => sp.GetService<RabbitMQClientFactory>(),
                exchange,
                timeout
            );
        }

        [Obsolete("Use PlugAndTrade.DieScheite.Client.Common.AddDieScheite(IServiceCollection, string, string, string) instead.")]
        public static IServiceCollection AddDieScheite(
                this IServiceCollection services,
                string serviceId,
                string serviceInstanceId,
                string serviceVersion)
        {
            return PlugAndTrade.DieScheite.Client.Common.DependencyInjection.AddDieScheite(
                services,
                serviceId,
                serviceInstanceId,
                serviceVersion
            );
        }
    }
}
