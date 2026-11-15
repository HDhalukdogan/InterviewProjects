using DoctorRedis.Domain.Entities;
using DoctorRedis.Domain.Events;
using DoctorRedis.Infrastructure.RabbitMQ;
using DoctorRedis.Persistence.Contexts;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace DoctorRedis.API.Consumers
{
    public class AppointmentCreatedConsumer(
        IRabbitMQConnection connection,
        IServiceScopeFactory scopeFactory,
        ILogger<AppointmentCreatedConsumer> logger) : BackgroundService
    {
        private IChannel? _channel;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var conn = await connection.GetConnectionAsync(stoppingToken);
            _channel = await conn.CreateChannelAsync(cancellationToken: stoppingToken);

            await _channel.ExchangeDeclareAsync(
                exchange: RabbitMQConstants.AppointmentExchange,
                type: ExchangeType.Direct,
                durable: true,
                cancellationToken: stoppingToken);

            await _channel.QueueDeclareAsync(
                queue: RabbitMQConstants.AppointmentCreatedQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                cancellationToken: stoppingToken);

            await _channel.QueueBindAsync(
                queue: RabbitMQConstants.AppointmentCreatedQueue,
                exchange: RabbitMQConstants.AppointmentExchange,
                routingKey: RabbitMQConstants.AppointmentCreatedRoutingKey,
                cancellationToken: stoppingToken);

            await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false, cancellationToken: stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (_, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var json = Encoding.UTF8.GetString(body);
                    var appointmentEvent = JsonSerializer.Deserialize<AppointmentCreatedEvent>(json);

                    if (appointmentEvent is not null)
                    {
                        logger.LogInformation("Randevu bildirimi tüketiliyor. Randevu ID: {Id}", appointmentEvent.AppointmentId);

                        using var scope = scopeFactory.CreateScope();
                        var dbContext = scope.ServiceProvider.GetRequiredService<DoctorRedisContext>();

                        var log = new NotificationLog
                        {
                            AppointmentId = appointmentEvent.AppointmentId,
                            Recipient = string.IsNullOrWhiteSpace(appointmentEvent.PatientEmail)
                                ? $"Hasta-{appointmentEvent.PatientId}"
                                : appointmentEvent.PatientEmail
                        };

                        await dbContext.NotificationLogs.AddAsync(log, stoppingToken);
                        await dbContext.SaveChangesAsync(stoppingToken);

                        logger.LogInformation("NotificationLog kaydedildi. Alıcı: {Recipient}", log.Recipient);
                    }

                    await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Randevu bildirimi işlenirken hata oluştu.");

                    await _channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true, cancellationToken: stoppingToken);
                }
            };

            await _channel.BasicConsumeAsync(
                queue: RabbitMQConstants.AppointmentCreatedQueue,
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_channel is not null)
            {
                await _channel.CloseAsync(cancellationToken);
                await _channel.DisposeAsync();
            }
            await base.StopAsync(cancellationToken);
        }
    }
}
