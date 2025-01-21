using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text;

namespace ProtectedService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RabbitMqController : ControllerBase
    {
        private readonly string _rabbitMqHost = "localhost";  // RabbitMQ host
        private readonly string _queueName = "task_queue";   // Queue name

        // POST api/rabbitmq/send
        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] string message)
        {
            var factory = new ConnectionFactory() { HostName = _rabbitMqHost };
            using (var connection = await factory.CreateConnectionAsync())
            using (var channel = await connection.CreateChannelAsync())
            {
                // Declare a queue (it must match the queue on the consumer side)
                await channel.QueueDeclareAsync(queue: _queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var body = Encoding.UTF8.GetBytes(message);

                // Send the message to the queue
                await channel.BasicPublishAsync(exchange: "",
                    routingKey: _queueName,
                    body: body);
                
                return Ok($"Message sent: {message}");
            }
        }
    }
}