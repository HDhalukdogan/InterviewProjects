using AmbulanceApp.Domain.Enums;

namespace AmbulanceApp.Service.DTOs;

public record CreateEmergencyCallDto(
    string CallerName,
    string CallerPhone,
    string IncidentAddress,
    decimal Latitude,
    decimal Longitude,
    string IncidentType,
    CallPriority Priority);
