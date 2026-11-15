using RabbitMQ.Client;

namespace DoctorRedis.Infrastructure.RabbitMQ
{
    public interface IRabbitMQConnection : IAsyncDisposable
    {
        Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken = default);
    }
}
