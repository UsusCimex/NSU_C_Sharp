using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MassTransit;

namespace ParticipantService
{
    class Program
    {
        public static void Main(string[] args)
        {
            var participantId = int.Parse(Environment.GetEnvironmentVariable("PARTICIPANT_ID") ?? "0");
            var participantName = Environment.GetEnvironmentVariable("PARTICIPANT_NAME") ?? "Unknown";
            var participantRole = Environment.GetEnvironmentVariable("PARTICIPANT_ROLE") ?? "Junior";

            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddMassTransit(x =>
                    {
                        x.AddConsumer<Consumers.HackathonStartConsumer>();

                        x.UsingRabbitMq((context, cfg) =>
                        {
                            cfg.Host("rabbitmq", "/", h =>
                            {
                                h.Username("guest");
                                h.Password("guest");
                            });

                            cfg.ReceiveEndpoint($"participant_{participantId}_queue", e =>
                            {
                                e.ConfigureConsumer<Consumers.HackathonStartConsumer>(context);
                            });
                        });
                    });

                    services.AddSingleton(new Shared.Models.Participant(participantId, participantName, participantRole));
                    services.AddHostedService<HostedServices.ParticipantHostedService>();
                })
                .Build()
                .Run();
        }
    }
}
