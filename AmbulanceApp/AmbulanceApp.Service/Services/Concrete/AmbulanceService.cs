using AmbulanceApp.Domain.Entities;
using AmbulanceApp.Domain.Enums;
using AmbulanceApp.Domain.Events;
using AmbulanceApp.Persistence.Contexts;
using AmbulanceApp.Service.DTOs;
using AmbulanceApp.Service.RabbitMQ;
using AmbulanceApp.Service.Redis;
using AmbulanceApp.Service.Services.Abstract;
using Microsoft.EntityFrameworkCore;

namespace AmbulanceApp.Service.Services.Concrete;

public class AmbulanceService(
    AmbulanceAppDbContext context,
    IRedisGeoLockService geoLockService,
    IRabbitMQService rabbitMQService) : IAmbulanceService
{
    public async Task<EmergencyCall> CreateEmergencyCallAsync(CreateEmergencyCallDto dto, CancellationToken ct = default)
    {
        var call = new EmergencyCall
        {
            CallCode = $"EMG-{DateTime.UtcNow.Year}-{Guid.NewGuid().ToString("N")[..5].ToUpper()}",
            CallerName = dto.CallerName,
            CallerPhone = dto.CallerPhone,
            IncidentAddress = dto.IncidentAddress,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            IncidentType = dto.IncidentType,
            Priority = dto.Priority,
            Status = CallStatus.Pending
        };

        await context.EmergencyCalls.AddAsync(call, ct);
        await context.SaveChangesAsync(ct);

        await rabbitMQService.PublishAsync(
            RabbitMQConstants.EmergencyCallCreatedQueue,
            new EmergencyCallCreatedEvent(call.Id, call.CallCode, call.IncidentType, (int)call.Priority, call.Latitude, call.Longitude),
            ct);

        return call;
    }

    public async Task<DispatchResponseDto> DispatchNearestAmbulanceAsync(int emergencyCallId, CancellationToken ct = default)
    {
        var call = await context.EmergencyCalls.FindAsync([emergencyCallId], ct)
            ?? throw new KeyNotFoundException("Acil durum çağrısı bulunamadı.");

        if (call.Status != CallStatus.Pending)
            throw new InvalidOperationException($"Çağrı sevk için uygun değil. Mevcut durum: {call.Status}");

        // 1. Redis GEO üzerinden en yakın müsait ambulans adaylarını çek
        var nearestCandidates = await geoLockService.FindNearestAmbulancesAsync(call.Latitude, call.Longitude);
        if (nearestCandidates.Count == 0)
            throw new InvalidOperationException("Yakın çevrede müsait ambulans bulunamadı.");

        // 2. Adaylar üzerinden sırayla Distributed Lock alarak ilk boş ambulansı yakala
        foreach (var (ambulanceId, distanceKm) in nearestCandidates)
        {
            var lockKey = RedisConstants.AmbulanceLockKey(ambulanceId);
            var lockToken = Guid.NewGuid().ToString();

            if (!await geoLockService.AcquireLockAsync(lockKey, lockToken, TimeSpan.FromSeconds(10)))
                continue; // Bu ambulans şu an başka bir operatör tarafından kilitlenmiş, bir sonrakine geç

            try
            {
                var ambulance = await context.Ambulances.FindAsync([ambulanceId], ct);
                if (ambulance is null || ambulance.Status != AmbulanceStatus.Available)
                {
                    await geoLockService.RemoveFromAvailableAsync(ambulanceId);
                    continue;
                }

                // Durum geçişleri (xmin Optimistic Concurrency korumalı)
                ambulance.Status = AmbulanceStatus.Dispatched;
                call.Status = CallStatus.Assigned;

                var dispatch = new Dispatch
                {
                    EmergencyCallId = call.Id,
                    AmbulanceId = ambulance.Id,
                    DispatchedAt = DateTime.UtcNow
                };

                await context.Dispatches.AddAsync(dispatch, ct);
                await context.SaveChangesAsync(ct);

                // Müsait ambulanslar GEO havuzundan çıkar
                await geoLockService.RemoveFromAvailableAsync(ambulance.Id);

                // Sevk event'ini yayınla (Consumer bunu yakalayıp DispatchAuditLogs tablosuna yazacak)
                await rabbitMQService.PublishAsync(
                    RabbitMQConstants.AmbulanceDispatchedQueue,
                    new AmbulanceDispatchedEvent(dispatch.Id, call.Id, call.CallCode, ambulance.Id, ambulance.CallSign, dispatch.DispatchedAt),
                    ct);

                return new DispatchResponseDto(
                    dispatch.Id,
                    call.CallCode,
                    ambulance.CallSign,
                    ambulance.PlateNumber,
                    ambulance.Status,
                    call.Status,
                    Math.Round(distanceKm, 2),
                    dispatch.DispatchedAt);
            }
            finally
            {
                await geoLockService.ReleaseLockAsync(lockKey, lockToken);
            }
        }

        throw new InvalidOperationException("Tüm yakın ambulanslar eşzamanlı olarak başka vakalara atandı. Lütfen tekrar deneyin.");
    }

    public async Task UpdateAmbulanceLocationAsync(int ambulanceId, UpdateLocationDto dto, CancellationToken ct = default)
    {
        var ambulance = await context.Ambulances.FindAsync([ambulanceId], ct)
            ?? throw new KeyNotFoundException("Ambulans bulunamadı.");

        ambulance.CurrentLatitude = dto.Latitude;
        ambulance.CurrentLongitude = dto.Longitude;
        await context.SaveChangesAsync(ct);

        // Eğer ambulans müsaitse Redis GEO koordinatını da güncelle
        if (ambulance.Status == AmbulanceStatus.Available)
        {
            await geoLockService.UpsertAvailableLocationAsync(ambulance.Id, dto.Latitude, dto.Longitude);
        }
    }

    public async Task MarkArrivedAtSceneAsync(int dispatchId, CancellationToken ct = default)
    {
        var dispatch = await context.Dispatches
            .Include(d => d.Ambulance)
            .Include(d => d.EmergencyCall)
            .FirstOrDefaultAsync(d => d.Id == dispatchId, ct)
            ?? throw new KeyNotFoundException("Sevk kaydı bulunamadı.");

        dispatch.ArrivedAtSceneAt = DateTime.UtcNow;
        dispatch.Ambulance.Status = AmbulanceStatus.OnScene;
        dispatch.EmergencyCall.Status = CallStatus.InProgress;

        await context.SaveChangesAsync(ct);

        await rabbitMQService.PublishAsync(
            RabbitMQConstants.DispatchStatusChangedQueue,
            new DispatchStatusChangedEvent(dispatch.Id, dispatch.EmergencyCall.CallCode, dispatch.Ambulance.CallSign, nameof(AmbulanceStatus.OnScene), DateTime.UtcNow),
            ct);
    }

    public async Task MarkDepartedToHospitalAsync(int dispatchId, int hospitalId, CancellationToken ct = default)
    {
        var dispatch = await context.Dispatches
            .Include(d => d.Ambulance)
            .Include(d => d.EmergencyCall)
            .FirstOrDefaultAsync(d => d.Id == dispatchId, ct)
            ?? throw new KeyNotFoundException("Sevk kaydı bulunamadı.");

        var hospitalExists = await context.Hospitals.AnyAsync(h => h.Id == hospitalId, ct);
        if (!hospitalExists)
            throw new KeyNotFoundException("Hedef hastane bulunamadı.");

        dispatch.HospitalId = hospitalId;
        dispatch.DepartedToHospitalAt = DateTime.UtcNow;
        dispatch.Ambulance.Status = AmbulanceStatus.Transporting;

        await context.SaveChangesAsync(ct);

        await rabbitMQService.PublishAsync(
            RabbitMQConstants.DispatchStatusChangedQueue,
            new DispatchStatusChangedEvent(dispatch.Id, dispatch.EmergencyCall.CallCode, dispatch.Ambulance.CallSign, nameof(AmbulanceStatus.Transporting), DateTime.UtcNow),
            ct);
    }

    public async Task CompleteDispatchAsync(int dispatchId, CompleteDispatchDto dto, CancellationToken ct = default)
    {
        var dispatch = await context.Dispatches
            .Include(d => d.Ambulance)
            .Include(d => d.EmergencyCall)
            .FirstOrDefaultAsync(d => d.Id == dispatchId, ct)
            ?? throw new KeyNotFoundException("Sevk kaydı bulunamadı.");

        dispatch.CompletedAt = DateTime.UtcNow;
        dispatch.CaseNotes = dto.CaseNotes;
        if (dto.HospitalId.HasValue)
            dispatch.HospitalId = dto.HospitalId.Value;

        dispatch.EmergencyCall.Status = CallStatus.Completed;
        dispatch.Ambulance.Status = AmbulanceStatus.Available;

        await context.SaveChangesAsync(ct);

        // Görev bitti: Ambulansı tekrar Redis GEO müsait havuzuna ekle
        await geoLockService.UpsertAvailableLocationAsync(
            dispatch.Ambulance.Id,
            dispatch.Ambulance.CurrentLatitude,
            dispatch.Ambulance.CurrentLongitude);

        await rabbitMQService.PublishAsync(
            RabbitMQConstants.DispatchStatusChangedQueue,
            new DispatchStatusChangedEvent(dispatch.Id, dispatch.EmergencyCall.CallCode, dispatch.Ambulance.CallSign, nameof(AmbulanceStatus.Available), DateTime.UtcNow),
            ct);
    }
}