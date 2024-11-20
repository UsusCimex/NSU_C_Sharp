using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MassTransit;

namespace HRManagerService
{
    class Program
    {
        public static void Main(string[] args)
        {
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddMassTransit(x =>
                    {
                        x.AddConsumer<Consumers.WishlistConsumer>();

                        x.UsingRabbitMq((context, cfg) =>
                        {
                            cfg.Host("rabbitmq", "/", h =>
                            {
                                h.Username("guest");
                                h.Password("guest");
                            });

                            cfg.ReceiveEndpoint("wishlist_queue", e =>
                            {
                                e.ConfigureConsumer<Consumers.WishlistConsumer>(context);
                            });
                        });
                    });

                    services.AddSingleton<Services.HRManagerService>();
                    services.AddHostedService<Workers.HRManagerWorker>();
                })
                .Build()
                .Run();
        }
    }
}
