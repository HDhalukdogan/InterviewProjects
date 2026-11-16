using PrescriptionApp.Application.RabbitMQ;
using PrescriptionApp.Domain.Events;

namespace PrescriptionApp.API.Consumers;

public class PrescriptionConsumer(
    IRabbitMQService rabbitMQService,
    IServiceScopeFactory scopeFactory,
    ILogger<PrescriptionConsumer> logger) : BackgroundService
{
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await rabbitMQService.ConsumeAsync<PrescriptionCreatedEvent>(
            RabbitMQConstants.PrescriptionCreatedQueue,
            async (message) =>
            {
                using var scope = scopeFactory.CreateScope();
                logger.LogInformation(
                    "[RABBITMQ - CREATED] Yeni reçete oluşturuldu! Kod: {Code} | Hasta: {Email} | Son Geçerlilik: {ExpiresAt}",
                    message.PrescriptionCode, message.PatientEmail, message.ExpiresAt);

                await Task.CompletedTask;
            },
            cancellationToken);

        await rabbitMQService.ConsumeAsync<PrescriptionDispensedEvent>(
            RabbitMQConstants.PrescriptionDispensedQueue,
            async (message) =>
            {
                using var scope = scopeFactory.CreateScope();
                logger.LogInformation(
                    "[RABBITMQ - DISPENSED] Reçete eczaneden teslim edildi! Kod: {Code} | Teslim Zamanı: {DispensedAt}",
                    message.PrescriptionCode, message.DispensedAt);

                await Task.CompletedTask;
            },
            cancellationToken);

        await base.StartAsync(cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.CompletedTask;
}