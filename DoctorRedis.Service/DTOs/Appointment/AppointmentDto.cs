namespace DoctorRedis.Service.DTOs
{
    public class AppointmentDto
    {
        public int Id { get; set; }
        public int SlotId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
    }
}
