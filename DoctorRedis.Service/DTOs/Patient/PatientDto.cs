namespace DoctorRedis.Service.DTOs
{
    public class PatientDto
    {
        public int Id { get; set; }
        public string IdentityNumber { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string? Email { get; set; }
        public DateTime BirthDate { get; set; }
    }
}
