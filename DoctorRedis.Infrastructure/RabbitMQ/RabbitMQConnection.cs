using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace DoctorRedis.Infrastructure.RabbitMQ
{
    public class RabbitMQConnection(IConfiguration configuration) : IRabbitMQConnection
    {
        private IConnection? _connection;
        private readonly SemaphoreSlim _lock = new(1, 1);

        public async Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken = default)
        {
            if (_connection is { IsOpen: true })
                return _connection;

            await _lock.WaitAsync(cancellationToken);
            try
            {
                if (_connection is { IsOpen: true })
                    return _connection;

                var factory = new ConnectionFactory
                {
                    HostName = configuration["RabbitMQ:Host"] ?? "localhost",
                    Port = int.Parse(configuration["RabbitMQ:Port"] ?? "5672"),
                    UserName = configuration["RabbitMQ:Username"] ?? "guest",
                    Password = configuration["RabbitMQ:Password"] ?? "guest"
                };

                _connection = await factory.CreateConnectionAsync(cancellationToken);
                return _connection;
            }
            finally
            {
                _lock.Release();
            }
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
