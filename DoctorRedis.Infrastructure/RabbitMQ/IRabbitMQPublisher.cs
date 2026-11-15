using System;
using System.Collections.Generic;
using System.Text;

namespace DoctorRedis.Infrastructure.RabbitMQ
{
    public interface IRabbitMQPublisher
    {
        Task PublishAsync<T>(T message, string exchange, string routingKey, CancellationToken cancellationToken = default);
    }
}
