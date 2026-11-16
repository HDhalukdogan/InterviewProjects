using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace AppointmentDbFirst.Infrastructure.RabbitMQ
{
    public class RabbitMQService : IRabbitMQService
    {
        private readonly ConnectionFactory _factory;
        private IConnection? _connection;
        private readonly SemaphoreSlim _connectionLock = new(1, 1);

        public RabbitMQService()
        {
            _factory = new ConnectionFactory
            {
                HostName = RabbitMQConstants.HostName,
                Port = int.Parse(RabbitMQConstants.Port),
                UserName = RabbitMQConstants.UserName,
                Password = RabbitMQConstants.Password
            };
        }

        private async Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken)
        {
            if (_connection is { IsOpen: true })
                return _connection;

            await _connectionLock.WaitAsync(cancellationToken);
            try
            {
                if (_connection is { IsOpen: true })
                    return _connection;

                _connection = await _factory.CreateConnectionAsync(cancellationToken);
                return _connection;
            }
            finally
            {
                _connectionLock.Release();
            }
        }

        public async Task PublishAsync<T>(string queueName, T message, CancellationToken cancellationToken = default) where T : class
        {
            var connection = await GetConnectionAsync(cancellationToken);
            await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

            // Kuyruk yoksa oluştur (durable: true -> RabbitMQ restart olsa bile kuyruk silinmez)
            await channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken);

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = new BasicProperties
            {
                Persistent = true // Mesajın diske yazılmasını sağlar
            };

            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: queueName,
                mandatory: false,
                basicProperties: properties,
                body: body,
                cancellationToken: cancellationToken);
        }

        public async Task ConsumeAsync<T>(string queueName, Func<T, Task> onMessageReceived, CancellationToken cancellationToken = default) where T : class
        {
            var connection = await GetConnectionAsync(cancellationToken);
            // Consume işlemi arka planda sürekli dinleme yapacağı için kanal 'await using' ile kapatılmamalıdır
            var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

            await channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken);

            // Aynı anda consumer'a en fazla 1 işlenmemiş mesaj gönder (Adil dağıtım)
            await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false, cancellationToken: cancellationToken);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (_, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var json = Encoding.UTF8.GetString(body);
                    var message = JsonSerializer.Deserialize<T>(json);

                    if (message is not null)
                    {
                        await onMessageReceived(message);
                    }

                    // İşlem hatasız tamamlandı, mesajı kuyruktan sil
                    await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false, cancellationToken: cancellationToken);
                }
                catch (Exception)
                {
                    // Hata durumunda mesajı tekrar kuyruğa al (requeue: true) veya DLQ yapısına yönlendir (requeue: false)
                    await channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true, cancellationToken: cancellationToken);
                }
            };

            await channel.BasicConsumeAsync(
                queue: queueName,
                autoAck: false, // Manuel Ack aktif
                consumer: consumer,
                cancellationToken: cancellationToken);
        }

        public async ValueTask DisposeAsync()
        {
            if (_connection is not null)
            {
                await _connection.CloseAsync();
                await _connection.DisposeAsync();
            }
            _connectionLock.Dispose();
        }
    }
}
