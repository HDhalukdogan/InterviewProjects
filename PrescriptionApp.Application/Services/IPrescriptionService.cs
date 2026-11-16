using PrescriptionApp.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrescriptionApp.Application.Services
{
    public interface IPrescriptionService
    {
        Task<PrescriptionResponseDto> CreatePrescriptionAsync(CreatePrescriptionDto dto, CancellationToken ct = default);
        Task<PrescriptionResponseDto?> GetByCodeAsync(string prescriptionCode, CancellationToken ct = default);
        Task<PrescriptionResponseDto> DispensePrescriptionAsync(string prescriptionCode, CancellationToken ct = default);
    }
}
