using AmbulanceApp.Domain.Enums;

namespace AmbulanceApp.Domain.Entities;

public class StaffMember : BaseEntity
{
    public string FullName { get; set; } = null!;
    public string RegistrationNo { get; set; } = null!;
    public StaffRole Role { get; set; }
    public int? AmbulanceId { get; set; }
    public bool IsActive { get; set; } = true;

    public Ambulance? Ambulance { get; set; }
}