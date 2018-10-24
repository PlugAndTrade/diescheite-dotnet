using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PlugAndTrade.DieScheite.Client.AspNetCore;
using PlugAndTrade.DieScheite.Client.Common;
using PlugAndTrade.DieScheite.Client.JsonConsole;
using PlugAndTrade.DieScheite.Client.RabbitMQ;
using PlugAndTrade.RabbitMQ;
using PlugAndTrade.TracingScope;
using PlugAndTrade.TracingScope.AspNetCore.DependencyInjection;

namespace PlugAndTrade.DieScheite.Client.Example.AspNetCore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                .AddTracingScope()
                .AddDieScheite("ExampleApi", "01", "0.1.0", sp => sp.GetService<ITracingScopeAccessor>())
                .AddDieScheiteJsonConsole()
                .AddSingleton<RabbitMQClientFactory>(sp => new RabbitMQClientFactory("localhost", 5672, "DieScheite-ExampleApi"))
                //.AddDieScheiteRabbitMQ(sp => sp.GetService<RabbitMQClientFactory>(), "diescheite")
                .AddMvc()
                .AddDieScheiteAspNetCore();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app
                .UseTracingScope()
                .UseDieScheite()
                .UseMvc(router =>
                {
                    router.MapRoute(
                        name: "Get routed",
                        template: "routed/{id}",
                        defaults: new { controller = "Routed", action = "GetSomeRandomMethod" },
                        constraints: new RouteValueDictionary(new { httpMethod = new HttpMethodRouteConstraint("GET") })
                    );
                });
        }
    }
}
