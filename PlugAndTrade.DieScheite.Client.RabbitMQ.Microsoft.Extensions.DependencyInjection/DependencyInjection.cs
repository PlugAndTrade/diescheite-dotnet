using System;
using Microsoft.Extensions.DependencyInjection;
using PlugAndTrade.RabbitMQ;
using PlugAndTrade.DieScheite.Client.Common;

namespace PlugAndTrade.DieScheite.Client.RabbitMQ
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDieScheiteRabbitMQ(
                this IServiceCollection services,
                Func<IServiceProvider, RabbitMQClientFactory> clientFactory,
                string exchange,
                int timeout = 60)
        {
            return services.AddSingleton<ILogger>(sp => new RabbitMQLogger(clientFactory(sp)
                .CreateProducer(exchange, timeout)
            ));
        }
    }
}
