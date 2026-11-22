namespace AmbulanceApp.Domain.Entities;

public class DispatchAuditLog : BaseEntity
{
    public int DispatchId { get; set; }
    public string EventType { get; set; } = null!;
    public string Payload { get; set; } = null!;

    public Dispatch Dispatch { get; set; } = null!;
}