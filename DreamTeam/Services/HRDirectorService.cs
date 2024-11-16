using System.Text;
using DreamTeam.Models;
using DreamTeam.Utilities;
using Newtonsoft.Json;
using RabbitMQ.Client;
using MySql.Data.MySqlClient;

namespace DreamTeam.Services
{
    public class HRDirectorService : DreamTeamService
    {
        private readonly IModel _channel;
        private readonly string _connectionString = "Server=mysql;Database=hackathon_db;User=root;Password=pass;";
        private List<Employee> _teamLeads;
        private List<Employee> _juniors;

        public HRDirectorService()
        {
            EnsureHackathonResultsExists();

            // Подключение к RabbitMQ
            var factory = new ConnectionFactory
            {
                HostName = "rabbitmq",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };

            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();

            // Читаем участников из файлов
            _teamLeads = ParticipantReader.ReadParticipants("TeamLeads.csv");
            _juniors = ParticipantReader.ReadParticipants("Juniors.csv");
        }

        private void EnsureHackathonResultsExists()
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            var command = new MySqlCommand(@"
                CREATE TABLE HackathonResults (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    HackathonId INT NOT NULL,
                    AverageHappiness DOUBLE NOT NULL,
                    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                )", connection);

            command.ExecuteNonQuery();
        }

        public void Start()
        {
            // Запуск веб-сервера для приема команд от HRManager
            var builder = WebApplication.CreateBuilder();
            builder.WebHost.UseUrls("http://*:80");
            builder.Services.AddLogging();
            var app = builder.Build();

            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            _ = app.MapPost("/api/teams", async (context) =>
            {
                logger.LogInformation("Получен запрос на /api/teams");
                var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<HRManagerData>(body)!;

                CalculateAndSaveHappiness(data.Teams, data.Wishlists);

                context.Response.StatusCode = 200;
            });

            // Запуск процесса инициирования хакатонов
            Task.Run(() => InitiateHackathons());

            app.Run();
        }

        private void InitiateHackathons()
        {
            for (int i = 1; i <= 100; i++)
            {
                Console.WriteLine($"HRDirector: Инициируем хакатон {i}");

                // Отправляем уведомление участникам о начале хакатона
                var hackathonStartMessage = new HackathonStartMessage
                {
                    HackathonId = i,
                    TeamLeads = _teamLeads,
                    Juniors = _juniors
                };

                var message = JsonConvert.SerializeObject(hackathonStartMessage);
                var body = Encoding.UTF8.GetBytes(message);

                var hackathonExchange = "hackathon_exchange";
                _channel.ExchangeDeclare(exchange: hackathonExchange, type: ExchangeType.Fanout);
                _channel.BasicPublish(exchange: hackathonExchange, routingKey: "", body: body);

                Console.WriteLine($"HRDirector: Отправлено уведомление о начале хакатона {i}");
            }
        }

        private void CalculateAndSaveHappiness(List<Team> teams, List<Wishlist> wishlists)
        {
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
            Console.WriteLine($"HRDirector: Хакатон {hackathonId}, Средняя счастливость: {averageHappiness}");

            SaveResultToDatabase(hackathonId, averageHappiness);
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

        private void SaveResultToDatabase(int hackathonId, double averageHappiness)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            var command = new MySqlCommand("INSERT INTO HackathonResults (HackathonId, AverageHappiness) VALUES (@HackathonId, @AverageHappiness)", connection);
            command.Parameters.AddWithValue("@HackathonId", hackathonId);
            command.Parameters.AddWithValue("@AverageHappiness", averageHappiness);

            command.ExecuteNonQuery();

            Console.WriteLine($"HRDirector: Результаты хакатона {hackathonId} сохранены в базу данных.");
        }

        public override string ToString()
        {
            return "HRDirector";
        }
    }
}
