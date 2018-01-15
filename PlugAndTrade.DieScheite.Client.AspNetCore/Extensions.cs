using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PlugAndTrade.DieScheite.Client.Common;
using PlugAndTrade.DieScheite.Client.Console;
using PlugAndTrade.DieScheite.Client.RabbitMQ;
using PlugAndTrade.RabbitMQ;

namespace PlugAndTrade.DieScheite.Client.AspNetCore
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseDieScheite(this IApplicationBuilder app)
        {
            app.UseMiddleware<Middleware>();
            return app;
        }

        public static IServiceCollection AddDieScheiteConsole(this IServiceCollection services)
        {
            services.AddSingleton<ILogger>(sp => new ConsoleLogger());
            return services;
        }

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

            services.AddSingleton<ILogger>(sp => new RabbitMQLogger(sp
                .GetService<RabbitMQClientFactory>()
                .CreateProducer(exchange, timeout)
            ));

            return services;
        }

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
