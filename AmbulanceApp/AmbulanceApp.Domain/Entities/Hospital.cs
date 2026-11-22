namespace AmbulanceApp.Domain.Entities;

public class Hospital : BaseEntity
{
    public string Name { get; set; } = null!;
    public string District { get; set; } = null!;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public bool HasTraumaCenter { get; set; }

    public ICollection<Dispatch> Dispatches { get; set; } = [];
}