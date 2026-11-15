namespace DoctorRedis.Service.DTOs
{
    public class PatientCreateDto
    {
        public string IdentityNumber { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string? Email { get; set; }
        public DateTime BirthDate { get; set; }
    }
}
