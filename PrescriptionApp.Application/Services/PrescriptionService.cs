using Microsoft.EntityFrameworkCore;
using PrescriptionApp.Application.DTOs;
using PrescriptionApp.Application.RabbitMQ;
using PrescriptionApp.Application.Redis;
using PrescriptionApp.Domain.Entities;
using PrescriptionApp.Domain.Enums;
using PrescriptionApp.Domain.Events;
using PrescriptionApp.Persistence.Contexts;

namespace PrescriptionApp.Application.Services
{
    public class PrescriptionService(
        PrescriptionDbContext context,
        IRedisCacheService cacheService,
        IRabbitMQService rabbitMQService) : IPrescriptionService
    {
        public async Task<PrescriptionResponseDto> CreatePrescriptionAsync(CreatePrescriptionDto dto, CancellationToken ct = default)
        {
            var doctor = await context.Doctors.FindAsync([dto.DoctorId], ct)
                ?? throw new KeyNotFoundException("Doktor bulunamadı.");

            var patient = await context.Patients.FindAsync([dto.PatientId], ct)
                ?? throw new KeyNotFoundException("Hasta bulunamadı.");

            var medicineIds = dto.Items.Select(i => i.MedicineId).Distinct().ToList();
            var medicines = await context.Medicines
                .Where(m => medicineIds.Contains(m.Id))
                .ToDictionaryAsync(m => m.Id, ct);

            if (medicines.Count != medicineIds.Count)
                throw new InvalidOperationException("Reçetedeki ilaçlardan biri veya birkaçı sistemde kayıtlı değil.");

            var code = $"RX-{DateTime.UtcNow.Year}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}";

            var prescription = new Prescription
            {
                PrescriptionCode = code,
                DoctorId = dto.DoctorId,
                PatientId = dto.PatientId,
                Status = PrescriptionStatus.Pending,
                ExpiresAt = DateTime.UtcNow.AddDays(dto.ValidDays > 0 ? dto.ValidDays : 3),
                Items = dto.Items.Select(i => new PrescriptionItem
                {
                    MedicineId = i.MedicineId,
                    Quantity = i.Quantity,
                    DosageInstructions = i.DosageInstructions
                }).ToList()
            };

            await context.Prescriptions.AddAsync(prescription, ct);
            await context.SaveChangesAsync(ct);

            var response = new PrescriptionResponseDto(
                prescription.Id,
                prescription.PrescriptionCode,
                doctor.FullName,
                patient.FullName,
                prescription.Status,
                prescription.ExpiresAt,
                prescription.DispensedAt,
                prescription.Items.Select(i =>
                {
                    var med = medicines[i.MedicineId];
                    return new PrescriptionItemResponseDto(
                        med.Id, med.Name, med.Barcode, i.Quantity, med.Price, i.DosageInstructions);
                }).ToList()
            );

            // 1. Redis Cache'e kaydet (Geçerlilik süresi kadar)
            var cacheDuration = prescription.ExpiresAt - DateTime.UtcNow;
            if (cacheDuration > TimeSpan.Zero)
                await cacheService.SetAsync(RedisConstants.PrescriptionCacheKey(code), response, cacheDuration);

            // 2. RabbitMQ'ya Event gönder
            await rabbitMQService.PublishAsync(
                RabbitMQConstants.PrescriptionCreatedQueue,
                new PrescriptionCreatedEvent(prescription.Id, code, patient.Id, patient.Email, prescription.ExpiresAt),
                ct);

            return response;
        }

        public async Task<PrescriptionResponseDto?> GetByCodeAsync(string prescriptionCode, CancellationToken ct = default)
        {
            var cacheKey = RedisConstants.PrescriptionCacheKey(prescriptionCode);

            // Önce Redis'e bak
            var cached = await cacheService.GetAsync<PrescriptionResponseDto>(cacheKey);
            if (cached is not null)
                return cached;

            // Yoksa DB'den çek
            var prescription = await context.Prescriptions
                .AsNoTracking()
                .Include(p => p.Doctor)
                .Include(p => p.Patient)
                .Include(p => p.Items)
                    .ThenInclude(i => i.Medicine)
                .FirstOrDefaultAsync(p => p.PrescriptionCode == prescriptionCode, ct);

            if (prescription is null)
                return null;

            var response = MapToResponse(prescription);

            var remaining = prescription.ExpiresAt - DateTime.UtcNow;
            if (remaining > TimeSpan.Zero)
                await cacheService.SetAsync(cacheKey, response, remaining);

            return response;
        }

        public async Task<PrescriptionResponseDto> DispensePrescriptionAsync(string prescriptionCode, CancellationToken ct = default)
        {
            var lockKey = RedisConstants.DispenseLockKey(prescriptionCode);
            var lockToken = Guid.NewGuid().ToString();

            // Aynı reçete koduna eşzamanlı iki eczaneden istek gelmesini engelle
            var acquired = await cacheService.AcquireLockAsync(lockKey, lockToken, TimeSpan.FromSeconds(15));
            if (!acquired)
                throw new InvalidOperationException("Bu reçete şu anda başka bir eczane tarafından işleniyor. Lütfen bekleyin.");

            try
            {
                var prescription = await context.Prescriptions
                    .Include(p => p.Doctor)
                    .Include(p => p.Patient)
                    .Include(p => p.Items)
                        .ThenInclude(i => i.Medicine)
                    .FirstOrDefaultAsync(p => p.PrescriptionCode == prescriptionCode, ct)
                    ?? throw new KeyNotFoundException("Reçete bulunamadı.");

                if (prescription.Status != PrescriptionStatus.Pending)
                    throw new InvalidOperationException($"Bu reçete işleme uygun değil. Mevcut durum: {prescription.Status}");

                if (prescription.ExpiresAt < DateTime.UtcNow)
                {
                    prescription.Status = PrescriptionStatus.Expired;
                    await context.SaveChangesAsync(ct);
                    await cacheService.RemoveAsync(RedisConstants.PrescriptionCacheKey(prescriptionCode));
                    throw new InvalidOperationException("Reçetenin geçerlilik süresi dolmuş.");
                }

                // İlaç stok kontrolü ve stok düşümü
                foreach (var item in prescription.Items)
                {
                    if (item.Medicine.StockQuantity < item.Quantity)
                        throw new InvalidOperationException($"'{item.Medicine.Name}' ilacı için stok yetersiz. Mevcut stok: {item.Medicine.StockQuantity}");

                    item.Medicine.StockQuantity -= item.Quantity;
                }

                prescription.Status = PrescriptionStatus.Dispensed;
                prescription.DispensedAt = DateTime.UtcNow;

                await context.SaveChangesAsync(ct);

                // Cache'i temizle (eski Pending durumu kalmasın)
                await cacheService.RemoveAsync(RedisConstants.PrescriptionCacheKey(prescriptionCode));

                // RabbitMQ'ya teslim edildi eventi gönder
                await rabbitMQService.PublishAsync(
                    RabbitMQConstants.PrescriptionDispensedQueue,
                    new PrescriptionDispensedEvent(prescription.Id, prescriptionCode, prescription.PatientId, prescription.DispensedAt.Value),
                    ct);

                return MapToResponse(prescription);
            }
            finally
            {
                await cacheService.ReleaseLockAsync(lockKey, lockToken);
            }
        }

        private static PrescriptionResponseDto MapToResponse(Prescription p) => new(
            p.Id,
            p.PrescriptionCode,
            p.Doctor.FullName,
            p.Patient.FullName,
            p.Status,
            p.ExpiresAt,
            p.DispensedAt,
            p.Items.Select(i => new PrescriptionItemResponseDto(
                i.MedicineId,
                i.Medicine.Name,
                i.Medicine.Barcode,
                i.Quantity,
                i.Medicine.Price,
                i.DosageInstructions)).ToList()
        );
    }
}
