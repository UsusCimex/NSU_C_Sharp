using System.Text;
using DreamTeam.Models;
using DreamTeam.Utilities;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using DreamTeam.Strategy;
using MySql.Data.MySqlClient;

namespace DreamTeam.Services
{
    public class HRManagerService : DreamTeamService
    {
        private readonly IModel _channel;
        private readonly Dictionary<int, List<Wishlist>> _wishlistsPerHackathon;
        private readonly object _lockObject = new object();
        private readonly string _connectionString = "Server=mysql;Database=hackathon_db;User ID=root;Password=pass;";
        private List<Employee> _teamLeads;
        private List<Employee> _juniors;

        public HRManagerService()
        {
            EnsureWishlistTableExists();

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

            _wishlistsPerHackathon = [];

            // Читаем участников из файлов
            _teamLeads = ParticipantReader.ReadParticipants("TeamLeads.csv");
            _juniors = ParticipantReader.ReadParticipants("Juniors.csv");

            // Подписка на получение вишлистов
            var wishlistsExchange = "wishlists_exchange";
            _channel.ExchangeDeclare(exchange: wishlistsExchange, type: ExchangeType.Fanout);
            var queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queue: queueName, exchange: wishlistsExchange, routingKey: "");

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += OnWishlistReceived!;
            _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        }

        private void EnsureWishlistTableExists()
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            var command = new MySqlCommand(@"
                CREATE TABLE IF NOT EXISTS Wishlists (
                    EmployeeId INT NOT NULL,
                    HackathonId INT NOT NULL,
                    DesiredEmployeeIds VARCHAR(255),
                    PRIMARY KEY (EmployeeId, HackathonId)
                )", connection);

            command.ExecuteNonQuery();
        }

        public void Start()
        {
            // Ожидание сообщений происходит асинхронно через RabbitMQ
        }

        private void OnWishlistReceived(object sender, BasicDeliverEventArgs e)
        {
            var message = Encoding.UTF8.GetString(e.Body.ToArray());
            var wishlist = JsonConvert.DeserializeObject<Wishlist>(message)!;

            Console.WriteLine($"HRManager: Получен вишлист от сотрудника {wishlist.EmployeeId} для хакатона {wishlist.HackathonId}");

            lock (_lockObject)
            {
                if (!_wishlistsPerHackathon.ContainsKey(wishlist.HackathonId))
                {
                    _wishlistsPerHackathon[wishlist.HackathonId] = new List<Wishlist>();
                }
                _wishlistsPerHackathon[wishlist.HackathonId].Add(wishlist);

                // Сохраняем вишлист в локальную базу данных
                SaveWishlistToDatabase(wishlist);

                // Проверяем, собраны ли все вишлисты для хакатона
                if (AllWishlistsCollected(wishlist.HackathonId))
                {
                    Console.WriteLine($"HRManager: Все вишлисты для хакатона {wishlist.HackathonId} собраны. Формируем команды.");
                    BuildTeamsAndSendToHRDirector(wishlist.HackathonId);
                }
            }
        }

        private bool AllWishlistsCollected(int hackathonId)
        {
            int totalParticipants = _teamLeads.Count + _juniors.Count;
            return _wishlistsPerHackathon[hackathonId].Count >= totalParticipants;
        }

        private void BuildTeamsAndSendToHRDirector(int hackathonId)
        {
            var wishlists = _wishlistsPerHackathon[hackathonId];

            var teamBuildingStrategy = new MegaTeamBuildingStrategy();
            var teams = teamBuildingStrategy.BuildTeams(
                hackathonId,
                _teamLeads,
                _juniors,
                wishlists.Where(w => _teamLeads.Any(tl => tl.Id == w.EmployeeId)),
                wishlists.Where(w => _juniors.Any(jr => jr.Id == w.EmployeeId))
            );

            var hrManagerData = new HRManagerData
            {
                Teams = teams.ToList(),
                Wishlists = wishlists
            };

            // Отправка данных HRDirector'у через HTTP
            try {
                using var httpClient = new HttpClient();
                var content = new StringContent(JsonConvert.SerializeObject(hrManagerData), Encoding.UTF8, "application/json");
                var response = httpClient.PostAsync("http://hrdirector:80/api/teams", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"HRManager: Отправлены команды для хакатона {hackathonId} HRDirector'у");
                }
                else
                {
                    Console.WriteLine($"HRManager: Ошибка при отправке данных HRDirector'у: {response.StatusCode}");
                }
            } catch(Exception ex) {
                Console.WriteLine($"Ошибка при отправке HTTP запроса: {ex.Message}");
            }

            // Удаляем записи о хакатоне, чтобы освободить память
            _wishlistsPerHackathon.Remove(hackathonId);
        }

        private void SaveWishlistToDatabase(Wishlist wishlist)
        {
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                connection.Open();

                var command = new MySqlCommand("INSERT INTO Wishlists (EmployeeId, HackathonId, DesiredEmployeeIds) VALUES (@EmployeeId, @HackathonId, @DesiredEmployeeIds)", connection);
                command.Parameters.AddWithValue("@EmployeeId", wishlist.EmployeeId);
                command.Parameters.AddWithValue("@HackathonId", wishlist.HackathonId);
                command.Parameters.AddWithValue("@DesiredEmployeeIds", string.Join(",", wishlist.DesiredEmployeeIds));

                command.ExecuteNonQuery();

                Console.WriteLine($"HRManager: Вишлист сотрудника {wishlist.EmployeeId} для хакатона {wishlist.HackathonId} сохранен базу данных.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении в базу данных: {ex.Message}");
            }
        }

        public override string ToString()
        {
            return "HRManager";
        }
    }
}
