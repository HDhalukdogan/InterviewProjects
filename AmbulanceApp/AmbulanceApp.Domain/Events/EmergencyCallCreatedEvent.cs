namespace AmbulanceApp.Domain.Events;

public record EmergencyCallCreatedEvent(
    int CallId,
    string CallCode,
    string IncidentType,
    int Priority,
    decimal Latitude,
    decimal Longitude);
