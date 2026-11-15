using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace DoctorRedis.Infrastructure.RabbitMQ
{
    public class RabbitMQPublisher(IRabbitMQConnection connection) : IRabbitMQPublisher
    {
        public async Task PublishAsync<T>(T message, string exchange, string routingKey, CancellationToken cancellationToken = default)
        {
            var conn = await connection.GetConnectionAsync(cancellationToken);
            await using var channel = await conn.CreateChannelAsync(cancellationToken: cancellationToken);

            await channel.ExchangeDeclareAsync(
                exchange: exchange,
                type: ExchangeType.Direct,
                durable: true,
                autoDelete: false,
                cancellationToken: cancellationToken);

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = new BasicProperties
            {
                Persistent = true,
                ContentType = "application/json"
            };

            await channel.BasicPublishAsync(
                exchange: exchange,
                routingKey: routingKey,
                mandatory: false,
                basicProperties: properties,
                body: body,
                cancellationToken: cancellationToken);
        }
    }
}
