namespace PrescriptionApp.Domain.Entities
{
    public class Doctor : BaseEntity
    {
        public string FullName { get; set; } = null!;
        public string DiplomaNo { get; set; } = null!;
        public string Branch { get; set; } = null!;

        public ICollection<Prescription> Prescriptions { get; set; } = [];
    }
}
