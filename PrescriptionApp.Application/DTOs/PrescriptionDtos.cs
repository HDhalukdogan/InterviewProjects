using PrescriptionApp.Domain.Enums;

namespace PrescriptionApp.Application.DTOs;

public record CreatePrescriptionItemDto(int MedicineId, int Quantity, string DosageInstructions);

public record CreatePrescriptionDto(
    int DoctorId,
    int PatientId,
    int ValidDays,
    List<CreatePrescriptionItemDto> Items);

public record PrescriptionItemResponseDto(
    int MedicineId,
    string MedicineName,
    string Barcode,
    int Quantity,
    decimal UnitPrice,
    string DosageInstructions);

public record PrescriptionResponseDto(
    int Id,
    string PrescriptionCode,
    string DoctorName,
    string PatientName,
    PrescriptionStatus Status,
    DateTime ExpiresAt,
    DateTime? DispensedAt,
    List<PrescriptionItemResponseDto> Items);