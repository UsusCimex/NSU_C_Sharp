using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Shared.Models;

namespace ParticipantService.Workers
{
    public class ParticipantHostedService : BackgroundService
    {
        private readonly ILogger<ParticipantHostedService> _logger;
        private readonly Participant _participant;
        private readonly IBusControl _bus;

        public ParticipantHostedService(ILogger<ParticipantHostedService> logger, Participant participant, IBusControl bus)
        {
            _logger = logger;
            _participant = participant;
            _bus = bus;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Participant {_participant.Name} is starting.");

            await _bus.StartAsync(stoppingToken);

            stoppingToken.Register(() => _bus.StopAsync());

            // Блокируем поток, чтобы сервис не завершился
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}
