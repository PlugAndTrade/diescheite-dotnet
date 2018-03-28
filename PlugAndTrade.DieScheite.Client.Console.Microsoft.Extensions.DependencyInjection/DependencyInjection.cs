using Microsoft.Extensions.DependencyInjection;
using PlugAndTrade.DieScheite.Client.Common;

namespace PlugAndTrade.DieScheite.Client.Console
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDieScheiteConsole(this IServiceCollection services) => services.AddSingleton<ILogger>(sp => new ConsoleLogger());
    }
}
