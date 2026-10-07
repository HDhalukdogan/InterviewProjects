namespace DoctorAppointmentBookingAPI.Entities
{
    public class Doctor
    {
        public Guid Id { get; set; }
        public required string FullName { get; set; }
        public required string Specialty { get; set; }
        public DateTime WorkingHoursStart { get; set; }
        public DateTime WorkingHoursEnd { get; set; }
        public int SlotDurationMinutes { get; set; } = 30;
        public ICollection<Appointment> Appointments { get; set; } = [];
    }
}
