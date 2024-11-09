using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

class Consumer
{
    public static void Main()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        string exchangeName = "fanout_exchange";
        channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout);

        var queueName = channel.QueueDeclare().QueueName;
        channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: "");

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($"[x] Получено сообщение: {message}");
        };

        channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

        Console.WriteLine("Нажмите [enter] для выхода.");
        Console.ReadLine();
    }
}
