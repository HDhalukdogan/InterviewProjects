namespace AppointmentDbFirst.Service.DTOs
{
    public class DoctorDto
    {
        public Guid Id { get; set; }

        public string FullName { get; set; } = null!;

        public string Specialty { get; set; } = null!;

        public DateTime WorkingHoursStart { get; set; }

        public DateTime WorkingHoursEnd { get; set; }

        public int SlotDurationMinutes { get; set; }
    }
}
