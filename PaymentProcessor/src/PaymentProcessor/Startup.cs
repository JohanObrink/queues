using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using ServiceStack.Redis;
using Microsoft.Extensions.PlatformAbstractions;
using PaymentProcessor.Services;

namespace PaymentProcessor
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
            services.AddSingleton<IConnectionFactory>(c => new ConnectionFactory() { HostName = "192.168.99.100" });
            services.AddSingleton<IRedisClientsManager>(c => new RedisManagerPool("192.168.99.100:6379"));
            services.AddSingleton(c => new PaymentStore(c.GetService<IRedisClientsManager>()));
            services.AddSingleton(c => new PaymentQueues(c.GetService<IConnectionFactory>(), c.GetService<PaymentStore>()));

            services.BuildServiceProvider().GetService<Services.PaymentQueues>().Connect();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseIISPlatformHandler();

            app.UseStaticFiles();

            app.UseMvc();
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
