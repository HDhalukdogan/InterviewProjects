using AmbulanceApp.Domain.Enums;

namespace AmbulanceApp.Service.DTOs;

public record DispatchResponseDto(
    int DispatchId,
    string CallCode,
    string AmbulanceCallSign,
    string PlateNumber,
    AmbulanceStatus AmbulanceStatus,
    CallStatus CallStatus,
    double DistanceKm,
    DateTime DispatchedAt);