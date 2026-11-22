using AmbulanceApp.Domain.Entities;
using AmbulanceApp.Domain.Enums;
using AmbulanceApp.Persistence.Contexts;
using AmbulanceApp.Service.DTOs;
using AmbulanceApp.Service.Redis;
using AmbulanceApp.Service.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AmbulanceApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AmbulancesController(
    IAmbulanceService ambulanceService,
    AmbulanceAppDbContext context,
    IRedisGeoLockService geoLockService) : ControllerBase
{
    [HttpPost("calls")]
    public async Task<IActionResult> CreateCall([FromBody] CreateEmergencyCallDto dto, CancellationToken ct)
    {
        var call = await ambulanceService.CreateEmergencyCallAsync(dto, ct);
        return Ok(call);
    }

    [HttpPost("calls/{callId:int}/dispatch-nearest")]
    public async Task<IActionResult> DispatchNearest(int callId, CancellationToken ct)
    {
        try
        {
            var result = await ambulanceService.DispatchNearestAmbulanceAsync(callId, ct);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict(new { error = "Ambulans eşzamanlı olarak başka bir operatör tarafından atandı. Tekrar deneyin." });
        }
    }

    [HttpPut("{ambulanceId:int}/location")]
    public async Task<IActionResult> UpdateLocation(int ambulanceId, [FromBody] UpdateLocationDto dto, CancellationToken ct)
    {
        try
        {
            await ambulanceService.UpdateAmbulanceLocationAsync(ambulanceId, dto, ct);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpPost("dispatches/{dispatchId:int}/arrived")]
    public async Task<IActionResult> MarkArrived(int dispatchId, CancellationToken ct)
    {
        try
        {
            await ambulanceService.MarkArrivedAtSceneAsync(dispatchId, ct);
            return Ok(new { message = "Olay yerine varış kaydedildi." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpPost("dispatches/{dispatchId:int}/depart-hospital/{hospitalId:int}")]
    public async Task<IActionResult> MarkDepartedToHospital(int dispatchId, int hospitalId, CancellationToken ct)
    {
        try
        {
            await ambulanceService.MarkDepartedToHospitalAsync(dispatchId, hospitalId, ct);
            return Ok(new { message = "Hastaneye nakil başlatıldı." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpPost("dispatches/{dispatchId:int}/complete")]
    public async Task<IActionResult> CompleteDispatch(int dispatchId, [FromBody] CompleteDispatchDto dto, CancellationToken ct)
    {
        try
        {
            await ambulanceService.CompleteDispatchAsync(dispatchId, dto, ct);
            return Ok(new { message = "Vaka tamamlandı, ambulans tekrar müsait havuzuna alındı." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    // Test için Ankara koordinatlarında 3 Ambulans + 1 Hastane oluşturur ve Redis GEO'ya yükler
    [HttpPost("seed")]
    public async Task<IActionResult> Seed(CancellationToken ct)
    {
        if (!await context.Hospitals.AnyAsync(ct))
        {
            await context.Hospitals.AddAsync(new Hospital
            {
                Name = "Ankara Bilkent Şehir Hastanesi",
                District = "Çankaya",
                Latitude = 39.903500m,
                Longitude = 32.753200m,
                HasTraumaCenter = true
            }, ct);
        }

        if (!await context.Ambulances.AnyAsync(ct))
        {
            var ambulances = new List<Ambulance>
            {
                // Kızılay Merkez (Olay yerine en yakın aday ~1.2 km)
                new() { PlateNumber = "06 AC 1121", CallSign = "ANK-112-01", Type = AmbulanceType.Emergency, CurrentLatitude = 39.920770m, CurrentLongitude = 32.854110m },
                // Bahçelievler (~4.5 km)
                new() { PlateNumber = "06 AC 1122", CallSign = "ANK-112-02", Type = AmbulanceType.IntensiveCare, CurrentLatitude = 39.921900m, CurrentLongitude = 32.818500m },
                // Etimesgut (~18 km)
                new() { PlateNumber = "06 AC 1123", CallSign = "ANK-112-03", Type = AmbulanceType.Emergency, CurrentLatitude = 39.946200m, CurrentLongitude = 32.677800m }
            };

            await context.Ambulances.AddRangeAsync(ambulances, ct);
            await context.SaveChangesAsync(ct);

            foreach (var amb in ambulances)
            {
                await geoLockService.UpsertAvailableLocationAsync(amb.Id, amb.CurrentLatitude, amb.CurrentLongitude);
            }
        }
        else
        {
            // DB'de varsa bile mevcut müsait ambulansları Redis GEO'ya senkronize et
            var available = await context.Ambulances
                .Where(a => a.Status == AmbulanceStatus.Available)
                .ToListAsync(ct);

            foreach (var amb in available)
            {
                await geoLockService.UpsertAvailableLocationAsync(amb.Id, amb.CurrentLatitude, amb.CurrentLongitude);
            }
        }

        return Ok(new
        {
            hospitals = await context.Hospitals.Select(h => new { h.Id, h.Name }).ToListAsync(ct),
            ambulances = await context.Ambulances.Select(a => new { a.Id, a.CallSign, a.PlateNumber, a.Status, a.CurrentLatitude, a.CurrentLongitude }).ToListAsync(ct)
        });
    }
}