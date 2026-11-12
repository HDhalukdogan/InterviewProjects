namespace DoctorRedis.Service.DTOs
{
    public class DoctorDto
    {
        public int Id { get; set; }
        public required string FullName { get; set; }
        public required string Specialty { get; set; }
    }
}
