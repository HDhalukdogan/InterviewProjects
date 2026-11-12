namespace DoctorRedis.Service.DTOs
{
    public class DoctorCreateDto
    {
        public required string FullName { get; set; }
        public required string Specialty { get; set; }
    }
}
