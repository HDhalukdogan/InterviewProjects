using AmbulanceApp.Domain.Entities;
using AmbulanceApp.Service.DTOs;

namespace AmbulanceApp.Service.Services.Abstract;

public interface IAmbulanceService
{
    // 1. Yeni 112 Acil Çağrısı Oluşturma
    Task<EmergencyCall> CreateEmergencyCallAsync(CreateEmergencyCallDto dto, CancellationToken ct = default);

    // 2. Olay Yerine En Yakın Müsait Ambulansı Bulup Sevk Etme (Redis GEO + Distributed Lock)
    Task<DispatchResponseDto> DispatchNearestAmbulanceAsync(int emergencyCallId, CancellationToken ct = default);

    // 3. Ambulans Canlı GPS Konum Güncelleme
    Task UpdateAmbulanceLocationAsync(int ambulanceId, UpdateLocationDto dto, CancellationToken ct = default);

    // 4. Operasyon Aşama Geçişleri (Olay Yerine Varış -> Hastaneye Hareket -> Vaka Tamamlama)
    Task MarkArrivedAtSceneAsync(int dispatchId, CancellationToken ct = default);
    Task MarkDepartedToHospitalAsync(int dispatchId, int hospitalId, CancellationToken ct = default);
    Task CompleteDispatchAsync(int dispatchId, CompleteDispatchDto dto, CancellationToken ct = default);
}