namespace AppointmentDbFirst.Infrastructure.RabbitMQ
{
    public interface IRabbitMQService : IAsyncDisposable
    {
        Task PublishAsync<T>(string queueName, T message, CancellationToken cancellationToken = default) where T : class;

        Task ConsumeAsync<T>(string queueName, Func<T, Task> onMessageReceived, CancellationToken cancellationToken = default) where T : class;
    }
}
