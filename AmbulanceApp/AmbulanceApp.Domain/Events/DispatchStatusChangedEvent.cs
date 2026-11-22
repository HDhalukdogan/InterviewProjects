namespace AmbulanceApp.Domain.Events;

public record DispatchStatusChangedEvent(
    int DispatchId,
    string CallCode,
    string CallSign,
    string NewStatus,
    DateTime OccurredAt);