using Shared.Models;
using Shared.Messages;
using MassTransit;
using HRDirectorService.Database;
using HRDirectorService.Models;
using HRDirectorService.Utilities;

namespace HRDirectorService.Services
{
    public class HRDirectorHostedService : IHostedService
    {
        private readonly ILogger<HRDirectorHostedService> _logger;
        private readonly IBusControl _bus;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly List<Employee> _teamLeads;
        private readonly List<Employee> _juniors;

        public HRDirectorHostedService(ILogger<HRDirectorHostedService> logger, IBusControl bus, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _bus = bus;
            _scopeFactory = scopeFactory;

            _logger.LogInformation("Start constructor");

            // Чтение участников из файлов
            _teamLeads = ParticipantReader.ReadParticipants("TeamLeads.csv", EmployeeRole.TeamLead);
            _juniors = ParticipantReader.ReadParticipants("Juniors.csv", EmployeeRole.Junior);

            var currentDirectory = Directory.GetCurrentDirectory();
            _logger.LogInformation($"Current Directory: {currentDirectory}");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("HRDirectorService starting.");

            // Запускаем длительные задачи в отдельном потоке
            Task.Run(() => InitiateHackathonsAsync(cancellationToken), cancellationToken);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("HRDirectorService stopping.");
            // Здесь можно добавить логику для отмены фоновых задач, если требуется
            return Task.CompletedTask;
        }

        private async Task InitiateHackathonsAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start hackathons");
            await Task.Delay(5000, cancellationToken); // 5 секунд до начала хакатонов

            for (int i = 1; i <= 100; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Hackathon initiation cancelled.");
                    break;
                }

                _logger.LogInformation($"HRDirector: Инициируем хакатон {i}");

                var hackathonStartMessage = new HackathonStartMessage
                {
                    HackathonId = i,
                    TeamLeads = _teamLeads,
                    Juniors = _juniors
                };

                await _bus.Publish(hackathonStartMessage);

                _logger.LogInformation($"HRDirector: Отправлено уведомление о начале хакатона {i}");
            }
        }

        public async Task ProcessHRManagerData(HRManagerData data)
        {
            if (data == null)
            {
                _logger.LogError("Получены пустые данные от HRManager.");
                return;
            }

            if ((data.Teams == null || !data.Teams.Any()) && (data.Wishlists == null || !data.Wishlists.Any()))
            {
                _logger.LogWarning("Получены пустые списки команд и пожеланий.");
                return;
            }

            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<HackathonDbContext>();

            CalculateAndSaveHappiness(dbContext, data.Teams ?? new List<Team>(), data.Wishlists ?? new List<Wishlist>());

            await Task.CompletedTask;
        }


        private void CalculateAndSaveHappiness(HackathonDbContext dbContext, List<Team> teams, List<Wishlist> wishlists)
        {
            if (teams == null || !teams.Any())
            {
                _logger.LogWarning("Список команд пуст. Невозможно рассчитать удовлетворённость.");
                return; // Прерываем выполнение метода, так как нет данных
            }

            int hackathonId = teams.First().HackathonId;
            double totalHappiness = 0;

            foreach (var team in teams)
            {
                int teamLeadScore = GetScore(team.TeamLead.Id, team.Junior.Id, hackathonId, wishlists);
                int juniorScore = GetScore(team.Junior.Id, team.TeamLead.Id, hackathonId, wishlists);

                double teamHappiness = 2.0 / ((1.0 / teamLeadScore) + (1.0 / juniorScore));
                totalHappiness += teamHappiness;
            }

            double averageHappiness = totalHappiness / teams.Count;
            _logger.LogInformation($"HRDirector: Хакатон {hackathonId}, Средняя удовлетворённость: {averageHappiness}");

            SaveResultToDatabase(dbContext, hackathonId, averageHappiness);
        }

        private int GetScore(int employeeId, int desiredId, int hackathonId, List<Wishlist> wishlists)
        {
            var wishlist = wishlists.FirstOrDefault(w => w.EmployeeId == employeeId && w.HackathonId == hackathonId);
            if (wishlist != null)
            {
                var index = Array.IndexOf(wishlist.DesiredEmployeeIds, desiredId);
                if (index >= 0)
                {
                    return 5 - index; // +5 за первое место, +4 за второе и т.д.
                }
            }
            return 1;
        }

        private void SaveResultToDatabase(HackathonDbContext dbContext, int hackathonId, double averageHappiness)
        {
            var result = new HackathonResult
            {
                HackathonId = hackathonId,
                AverageHappiness = averageHappiness,
                CreatedAt = DateTime.UtcNow
            };

            dbContext.HackathonResults.Add(result);
            dbContext.SaveChanges();

            _logger.LogInformation($"HRDirector: Результаты хакатона {hackathonId} сохранены в базу данных.");
        }
    }
}
