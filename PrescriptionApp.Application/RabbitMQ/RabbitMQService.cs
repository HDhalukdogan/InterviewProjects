using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace PrescriptionApp.Application.RabbitMQ
{
    public class RabbitMQService : IRabbitMQService
    {
        private readonly ConnectionFactory _factory = new()
        {
            HostName = RabbitMQConstants.HostName,
            Port = RabbitMQConstants.Port,
            UserName = RabbitMQConstants.UserName,
            Password = RabbitMQConstants.Password
        };

        private IConnection? _connection;
        private readonly SemaphoreSlim _lock = new(1, 1);

        private async Task<IConnection> GetConnectionAsync(CancellationToken ct)
        {
            if (_connection is { IsOpen: true }) return _connection;

            await _lock.WaitAsync(ct);
            try
            {
                if (_connection is { IsOpen: true }) return _connection;
                _connection = await _factory.CreateConnectionAsync(ct);
                return _connection;
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task PublishAsync<T>(string queueName, T message, CancellationToken cancellationToken = default) where T : class
        {
            var connection = await GetConnectionAsync(cancellationToken);
            await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

            await channel.QueueDeclareAsync(queueName, durable: true, exclusive: false, autoDelete: false, cancellationToken: cancellationToken);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            var props = new BasicProperties { Persistent = true };

            await channel.BasicPublishAsync(string.Empty, queueName, mandatory: false, props, body, cancellationToken);
        }

        public async Task ConsumeAsync<T>(string queueName, Func<T, Task> onMessageReceived, CancellationToken cancellationToken = default) where T : class
        {
            var connection = await GetConnectionAsync(cancellationToken);
            var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

            await channel.QueueDeclareAsync(queueName, durable: true, exclusive: false, autoDelete: false, cancellationToken: cancellationToken);
            await channel.BasicQosAsync(0, 1, false, cancellationToken);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (_, ea) =>
            {
                try
                {
                    var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var data = JsonSerializer.Deserialize<T>(json);
                    if (data is not null)
                        await onMessageReceived(data);

                    await channel.BasicAckAsync(ea.DeliveryTag, false, cancellationToken);
                }
                catch
                {
                    await channel.BasicNackAsync(ea.DeliveryTag, false, requeue: true, cancellationToken);
                }
            };

            await channel.BasicConsumeAsync(queueName, autoAck: false, consumer, cancellationToken);
        }

        public async ValueTask DisposeAsync()
        {
            if (_connection is not null)
            {
                await _connection.CloseAsync();
                await _connection.DisposeAsync();
            }
            _lock.Dispose();
        }
    }
}
