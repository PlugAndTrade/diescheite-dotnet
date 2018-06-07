using System;
using Microsoft.Extensions.DependencyInjection;
using PlugAndTrade.DieScheite.Client.Common;

namespace PlugAndTrade.DieScheite.Client.JsonConsole
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDieScheiteJsonConsole(this IServiceCollection services)
        {
            return services.AddSingleton<ILogger>(sp => new JsonConsoleLogger());
        }
    }
}
