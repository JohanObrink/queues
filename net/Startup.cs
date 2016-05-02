namespace NancyApplication
{
    using Microsoft.AspNet.Builder;
    using Nancy.Owin;
    using RabbitMQ.Client;
 
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UseOwin(x => x.UseNancy());
            
            var factory = new ConnectionFactory();
            factory.Uri = "amqp://guest:guest@rabbitmq:5672/";

            IConnection conn = factory.CreateConnection();
            var channel = conn.CreateModel();
            
        }

        // Entry point for the application.
        public static void Main(string[] args) => Microsoft.AspNet.Hosting.WebApplication.Run<Startup>(args);
    }
}
