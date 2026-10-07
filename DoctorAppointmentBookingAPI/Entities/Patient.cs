namespace DoctorAppointmentBookingAPI.Entities
{
    public class Patient
    {
        public Guid Id { get; set; }
        public required string FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public ICollection<Appointment> Appointments { get; set; } = [];
    }
}
