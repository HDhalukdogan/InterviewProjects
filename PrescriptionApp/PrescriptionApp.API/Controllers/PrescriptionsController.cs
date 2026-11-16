using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrescriptionApp.Application.DTOs;
using PrescriptionApp.Application.Services;
using PrescriptionApp.Domain.Entities;
using PrescriptionApp.Persistence.Contexts;

namespace PrescriptionApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PrescriptionsController(
    IPrescriptionService prescriptionService,
    PrescriptionDbContext context) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePrescriptionDto dto, CancellationToken ct)
    {
        try
        {
            var result = await prescriptionService.CreatePrescriptionAsync(dto, ct);
            return CreatedAtAction(nameof(GetByCode), new { code = result.PrescriptionCode }, result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{code}")]
    public async Task<IActionResult> GetByCode(string code, CancellationToken ct)
    {
        var result = await prescriptionService.GetByCodeAsync(code, ct);
        return result is null ? NotFound(new { error = "Reçete bulunamadı." }) : Ok(result);
    }

    [HttpPost("{code}/dispense")]
    public async Task<IActionResult> Dispense(string code, CancellationToken ct)
    {
        try
        {
            var result = await prescriptionService.DispensePrescriptionAsync(code, ct);
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
            return Conflict(new { error = "Stok eşzamanlı olarak başka bir işlem tarafından güncellendi. Lütfen tekrar deneyin." });
        }
    }

    // Hızlı test için örnek Doktor, Hasta ve İlaç verisi oluşturur
    [HttpPost("seed")]
    public async Task<IActionResult> SeedSampleData(CancellationToken ct)
    {
        if (!await context.Doctors.AnyAsync(ct))
        {
            await context.Doctors.AddAsync(new Doctor
            {
                FullName = "Dr. Ahmet Yılmaz",
                DiplomaNo = "DIP-2026-001",
                Branch = "Dahiliye"
            }, ct);
        }

        if (!await context.Patients.AnyAsync(ct))
        {
            await context.Patients.AddAsync(new Patient
            {
                FullName = "Mehmet Demir",
                IdentityNumber = "12345678901",
                Email = "mehmet.demir@example.com"
            }, ct);
        }

        if (!await context.Medicines.AnyAsync(ct))
        {
            await context.Medicines.AddRangeAsync([
                new Medicine { Barcode = "8699546010011", Name = "Parol 500 mg 20 Tablet", StockQuantity = 50, Price = 85.50m, IsSpecialControl = false },
                new Medicine { Barcode = "8699546020022", Name = "Augmentin BID 1000 mg 14 Tablet", StockQuantity = 10, Price = 240.00m, IsSpecialControl = false }
            ], ct);
        }

        await context.SaveChangesAsync(ct);

        return Ok(new
        {
            doctors = await context.Doctors.Select(d => new { d.Id, d.FullName }).ToListAsync(ct),
            patients = await context.Patients.Select(p => new { p.Id, p.FullName }).ToListAsync(ct),
            medicines = await context.Medicines.Select(m => new { m.Id, m.Name, m.StockQuantity }).ToListAsync(ct)
        });
    }
}