using AmbulanceApp.Domain.Enums;

namespace AmbulanceApp.Domain.Entities;

public class EmergencyCall : BaseEntity
{
    public string CallCode { get; set; } = null!;
    public string CallerName { get; set; } = null!;
    public string CallerPhone { get; set; } = null!;
    public string IncidentAddress { get; set; } = null!;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string IncidentType { get; set; } = null!;
    public CallPriority Priority { get; set; }
    public CallStatus Status { get; set; } = CallStatus.Pending;

    public ICollection<Dispatch> Dispatches { get; set; } = [];
}