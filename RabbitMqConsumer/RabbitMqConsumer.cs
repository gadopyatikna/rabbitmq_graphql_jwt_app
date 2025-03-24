using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace RabbitMqConsumer
{
    // docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:management
    public class RabbitMqConsumer : BackgroundService
    {
        private readonly string _rabbitMqHost;
        private readonly string _queueName;

        public RabbitMqConsumer(string host, string queueName)
        {
            _rabbitMqHost = host ?? "localhost";
            _queueName = queueName ?? "task_queue";
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory() { HostName = _rabbitMqHost };

            var connection = await factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine($"Received message: {message}");

                // Simulate work (e.g., process the message)
                Thread.Sleep(1000); // Simulating background work

                // Acknowledge that the message was processed
                await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            await channel.BasicConsumeAsync(queue: _queueName,
                autoAck: false,
                consumer: consumer);
        }
    }
}