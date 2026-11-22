namespace AmbulanceApp.Domain.Entities;

public class Dispatch : BaseEntity
{
    public int EmergencyCallId { get; set; }
    public int AmbulanceId { get; set; }
    public int? HospitalId { get; set; }
    public DateTime DispatchedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ArrivedAtSceneAt { get; set; }
    public DateTime? DepartedToHospitalAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? CaseNotes { get; set; }

    public EmergencyCall EmergencyCall { get; set; } = null!;
    public Ambulance Ambulance { get; set; } = null!;
    public Hospital? Hospital { get; set; }
    public ICollection<DispatchAuditLog> AuditLogs { get; set; } = [];
}