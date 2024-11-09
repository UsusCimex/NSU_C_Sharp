using System.Text;
using DreamTeam.Models;
using DreamTeam.Utilities;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Data.SqlClient;
using DreamTeam.Strategy;

namespace DreamTeam.Services
{
    public class HRManagerService : DreamTeamService
    {
        private readonly IModel _channel;
        private readonly Dictionary<int, List<Wishlist>> _wishlistsPerHackathon;
        private readonly object _lockObject = new object();
        private readonly string _connectionString = "Server=localhost;Database=hrmanager_db;User Id=sa;Password=Your_password123;";
        private List<Employee> _teamLeads;
        private List<Employee> _juniors;

        public HRManagerService()
        {
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
            var wishlistQueue = "wishlist_queue";
            _channel.QueueDeclare(queue: wishlistQueue, durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += OnWishlistReceived!;
            _channel.BasicConsume(queue: wishlistQueue, autoAck: true, consumer: consumer);
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

            var teamBuildingStrategy = new RandomTeamBuildingStrategy();
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
            using var httpClient = new HttpClient();
            var content = new StringContent(JsonConvert.SerializeObject(hrManagerData), Encoding.UTF8, "application/json");
            var response = httpClient.PostAsync("http://hrdirector-service/api/teams", content).Result;

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"HRManager: Отправлены команды для хакатона {hackathonId} HRDirector'у");
            }
            else
            {
                Console.WriteLine($"HRManager: Ошибка при отправке данных HRDirector'у: {response.StatusCode}");
            }

            // Удаляем записи о хакатоне, чтобы освободить память
            _wishlistsPerHackathon.Remove(hackathonId);
        }

        private void SaveWishlistToDatabase(Wishlist wishlist)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            var command = new SqlCommand("INSERT INTO Wishlists (EmployeeId, HackathonId, DesiredEmployeeIds) VALUES (@EmployeeId, @HackathonId, @DesiredEmployeeIds)", connection);
            command.Parameters.AddWithValue("@EmployeeId", wishlist.EmployeeId);
            command.Parameters.AddWithValue("@HackathonId", wishlist.HackathonId);
            command.Parameters.AddWithValue("@DesiredEmployeeIds", string.Join(",", wishlist.DesiredEmployeeIds));

            command.ExecuteNonQuery();

            Console.WriteLine($"HRManager: Вишлист сотрудника {wishlist.EmployeeId} для хакатона {wishlist.HackathonId} сохранен в локальную базу данных.");
        }

        public override string ToString()
        {
            return "HRManager";
        }
    }
}
