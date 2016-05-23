using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PaymentProcessor.Services;
using RabbitMQ.Client;
using System.Net;

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
            services.AddSingleton<RethinkDb.IConnectionFactory>(c => new RethinkDb.ConnectionFactories.DefaultConnectionFactory(new EndPoint[] { new IPEndPoint(IPAddress.Parse("192.168.99.100"), 28015) }));
            services.AddSingleton(c => new PaymentStore(c.GetService<RethinkDb.IConnectionFactory>(), "queues"));
            services.AddSingleton(c => new PaymentQueues(c.GetService<IConnectionFactory>(), c.GetService<PaymentStore>()));

            services.BuildServiceProvider().GetService<PaymentQueues>().Connect();
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
