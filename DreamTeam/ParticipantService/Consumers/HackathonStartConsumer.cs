using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Messages;
using Shared.Models;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace ParticipantService.Consumers
{
    public class HackathonStartConsumer : IConsumer<HackathonStartMessage>
    {
        private readonly ILogger<HackathonStartConsumer> _logger;
        private readonly Participant _participant;
        private readonly IBus _bus;

        public HackathonStartConsumer(ILogger<HackathonStartConsumer> logger, Participant participant, IBus bus)
        {
            _logger = logger;
            _participant = participant;
            _bus = bus;
        }

        public async Task Consume(ConsumeContext<HackathonStartMessage> context)
        {
            var message = context.Message;
            _logger.LogInformation($"[{_participant.Role}] {_participant.Name} получил уведомление о начале хакатона {message.HackathonId}");

            // Получаем список сотрудников противоположной роли
            var otherParticipants = _participant.Role == EmployeeRole.Junior ? message.TeamLeads : message.Juniors;

            // Генерируем вишлист
            var random = new Random();
            var desiredEmployees = otherParticipants.OrderBy(x => random.Next()).Take(5).Select(x => x.Id).ToArray();

            var wishlist = new Wishlist(_participant.Id, message.HackathonId, desiredEmployees);

            // Отправляем вишлист через MassTransit
            await _bus.Publish(wishlist);

            _logger.LogInformation($"[{_participant.Role}] {_participant.Name} отправил вишлист для хакатона {message.HackathonId}");
        }
    }
}
