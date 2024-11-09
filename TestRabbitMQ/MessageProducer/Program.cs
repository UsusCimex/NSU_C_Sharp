using System;
using System.Text;
using RabbitMQ.Client;

class Producer
{
    public static void Main()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        string exchangeName = "fanout_exchange";
        channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout);

        string message = "Привет, мир!";
        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(exchange: exchangeName, routingKey: "", basicProperties: null, body: body);
        Console.WriteLine($"[x] Отправлено сообщение: {message}");
    }
}
