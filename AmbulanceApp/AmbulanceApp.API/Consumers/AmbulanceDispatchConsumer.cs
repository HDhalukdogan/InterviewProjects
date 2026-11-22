using System.Text.Json;
using AmbulanceApp.Domain.Entities;
using AmbulanceApp.Domain.Events;
using AmbulanceApp.Persistence.Contexts;
using AmbulanceApp.Service.RabbitMQ;

namespace AmbulanceApp.API.Consumers;

public class AmbulanceDispatchConsumer(
    IRabbitMQService rabbitMQService,
    IServiceScopeFactory scopeFactory,
    ILogger<AmbulanceDispatchConsumer> logger) : BackgroundService
{
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        // 1. Yeni 112 Çağrısı Bildirimi
        await rabbitMQService.ConsumeAsync<EmergencyCallCreatedEvent>(
            RabbitMQConstants.EmergencyCallCreatedQueue,
            async (message) =>
            {
                logger.LogWarning(
                    "[112 YENİ ÇAĞRI] Kod: {CallCode} | Vaka: {Type} | Öncelik: {Priority} | Konum: ({Lat}, {Lng})",
                    message.CallCode, message.IncidentType, message.Priority, message.Latitude, message.Longitude);

                await Task.CompletedTask;
            },
            cancellationToken);

        // 2. Ambulans Sevk Edildi -> DispatchAuditLogs (jsonb) Kaydı
        await rabbitMQService.ConsumeAsync<AmbulanceDispatchedEvent>(
            RabbitMQConstants.AmbulanceDispatchedQueue,
            async (message) =>
            {
                using var scope = scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AmbulanceAppDbContext>();

                var auditLog = new DispatchAuditLog
                {
                    DispatchId = message.DispatchId,
                    EventType = nameof(AmbulanceDispatchedEvent),
                    Payload = JsonSerializer.Serialize(message)
                };

                await dbContext.DispatchAuditLogs.AddAsync(auditLog, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);

                logger.LogInformation(
                    "[SEVK ONAYLANDI] Çağrı: {CallCode} -> Ambulans: {CallSign} (AuditLog ID: {LogId})",
                    message.CallCode, message.CallSign, auditLog.Id);
            },
            cancellationToken);

        // 3. Operasyon Durum Geçişleri -> DispatchAuditLogs (jsonb) Kaydı
        await rabbitMQService.ConsumeAsync<DispatchStatusChangedEvent>(
            RabbitMQConstants.DispatchStatusChangedQueue,
            async (message) =>
            {
                using var scope = scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AmbulanceAppDbContext>();

                var auditLog = new DispatchAuditLog
                {
                    DispatchId = message.DispatchId,
                    EventType = $"{nameof(DispatchStatusChangedEvent)}:{message.NewStatus}",
                    Payload = JsonSerializer.Serialize(message)
                };

                await dbContext.DispatchAuditLogs.AddAsync(auditLog, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);

                logger.LogInformation(
                    "[DURUM GÜNCELLEME] Çağrı: {CallCode} | Ambulans: {CallSign} -> Yeni Durum: {Status}",
                    message.CallCode, message.CallSign, message.NewStatus);
            },
            cancellationToken);

        await base.StartAsync(cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.CompletedTask;
}