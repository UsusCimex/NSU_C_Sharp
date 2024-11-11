using System.Text;
using DreamTeam.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace DreamTeam.Services
{
    public class ParticipantService : DreamTeamService
    {
        private readonly Participant _participant;
        private readonly IModel _channel;

        public ParticipantService(Participant participant)
        {
            _participant = participant;

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

            // Подписка на уведомления о начале хакатона
            var hackathonExchange = "hackathon_exchange";
            _channel.ExchangeDeclare(exchange: hackathonExchange, type: ExchangeType.Fanout);
            var queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queue: queueName, exchange: hackathonExchange, routingKey: "");

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += OnHackathonStarted!;
            _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        }

        public void Start()
        {
            // Ожидание сообщений происходит асинхронно через RabbitMQ
        }

        private void OnHackathonStarted(object sender, BasicDeliverEventArgs e)
        {
            var message = Encoding.UTF8.GetString(e.Body.ToArray());
            var hackathonStart = JsonConvert.DeserializeObject<HackathonStartMessage>(message)!;

            Console.WriteLine($"[{_participant.Role}] {_participant.Name} получил уведомление о начале хакатона {hackathonStart.HackathonId}");

            // Получаем список участников противоположной роли
            var otherParticipants = _participant.Role == EmployeeRole.Junior
                ? hackathonStart.TeamLeads
                : hackathonStart.Juniors;

            // Генерируем и отправляем вишлист
            GenerateAndSendWishlist(hackathonStart.HackathonId, otherParticipants);
        }

        private void GenerateAndSendWishlist(int hackathonId, List<Employee> otherParticipants)
        {
            var random = new Random();
            var desiredEmployees = otherParticipants
                .OrderBy(_ => random.Next())
                .Take(5)
                .Select(e => e.Id)
                .ToArray();

            var wishlist = new Wishlist(_participant.Id, hackathonId, desiredEmployees);

            // Отправка вишлиста в RabbitMQ
            var wishlistsExchange = "wishlists_exchange";
            var queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queue: queueName, exchange: wishlistsExchange, routingKey: "");
            var message = JsonConvert.SerializeObject(wishlist);
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: wishlistsExchange, routingKey: "", basicProperties: null, body: body);

            Console.WriteLine($"[{_participant.Role}] {_participant.Name} отправил вишлист для хакатона {hackathonId}");
        }

        public override string ToString()
        {
            return _participant.ToString()!;
        }
    }
}
