using CartDbFirstApp.Service.DTOs;
using CartDbFirstApp.Service.RabbitMQ;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace CartDbFirstApp.API.Consumers
{
    public class CartConsumer : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = RabbitMQConstants.RabbitMQHostName,
                Port = int.Parse(RabbitMQConstants.RabbitMQPort),
                UserName = RabbitMQConstants.RabbitMQUserName,
                Password = RabbitMQConstants.RabbitMQPassword,
            };

            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queue: RabbitMQConstants.RabbitMQCartQueue, durable: true, exclusive: false, autoDelete: false, cancellationToken: stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (_, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var json = Encoding.UTF8.GetString(body);
                    var carts = JsonSerializer.Deserialize<IEnumerable<CartDto>>(json);
                    await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);

                }
                catch (Exception)
                {
                    await channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false, cancellationToken: stoppingToken);
                }
            };
            await channel.BasicConsumeAsync(queue: RabbitMQConstants.RabbitMQCartQueue, autoAck: false, consumer);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}
