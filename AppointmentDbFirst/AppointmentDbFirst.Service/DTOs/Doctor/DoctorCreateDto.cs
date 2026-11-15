namespace AppointmentDbFirst.Service.DTOs.Doctor
{
    public class DoctorCreateDto
    {
        public string FullName { get; set; } = null!;

        public string Specialty { get; set; } = null!;

        public DateTime WorkingHoursStart { get; set; }

        public DateTime WorkingHoursEnd { get; set; }

        public int SlotDurationMinutes { get; set; }
    }
}
