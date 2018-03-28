using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PlugAndTrade.DieScheite.Client.AspNetCore;
using PlugAndTrade.DieScheite.Client.Common;
using PlugAndTrade.DieScheite.Client.Console;
using PlugAndTrade.DieScheite.Client.RabbitMQ;
using PlugAndTrade.RabbitMQ;

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
                .AddDieScheite("ExampleApi", "01", "0.1.0")
                .AddDieScheiteConsole()
                .AddSingleton<RabbitMQClientFactory>(sp => new RabbitMQClientFactory("localhost", 5672, "DieScheite-ExampleApi"))
                .AddDieScheiteRabbitMQ(sp => sp.GetService<RabbitMQClientFactory>(), "diescheite")
                .AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDieScheite();
            app.UseMvc();
        }
    }
}
