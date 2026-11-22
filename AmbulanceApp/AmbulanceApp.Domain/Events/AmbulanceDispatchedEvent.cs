namespace AmbulanceApp.Domain.Events;

public record AmbulanceDispatchedEvent(
    int DispatchId,
    int EmergencyCallId,
    string CallCode,
    int AmbulanceId,
    string CallSign,
    DateTime DispatchedAt);

