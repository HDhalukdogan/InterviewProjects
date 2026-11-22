using AmbulanceApp.Domain.Enums;

namespace AmbulanceApp.Domain.Entities;

public class Ambulance : BaseEntity
{
    public string PlateNumber { get; set; } = null!;
    public string CallSign { get; set; } = null!;
    public AmbulanceType Type { get; set; }
    public AmbulanceStatus Status { get; set; } = AmbulanceStatus.Available;
    public decimal CurrentLatitude { get; set; }
    public decimal CurrentLongitude { get; set; }
    public uint RowVersion { get; set; }

    public ICollection<StaffMember> StaffMembers { get; set; } = [];
    public ICollection<Dispatch> Dispatches { get; set; } = [];
}