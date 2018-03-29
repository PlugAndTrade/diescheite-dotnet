using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using PlugAndTrade.DieScheite.Client.Common;

namespace PlugAndTrade.DieScheite.Client.Console
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDieScheiteConsole(this IServiceCollection services) =>
            services.AddSingleton<ILogger>(sp => new ConsoleLogger());
        public static IServiceCollection AddDieScheiteConsole(this IServiceCollection services, IReadOnlyCollection<Func<LogEntry, bool>> filters) =>
            services.AddSingleton<ILogger>(sp => new ConsoleLogger(filters));
        public static IServiceCollection AddDieScheiteConsole(this IServiceCollection services, LogEntryLevel minLevel) =>
            services.AddSingleton<ILogger>(sp => new ConsoleLogger(minLevel));
    }
}
