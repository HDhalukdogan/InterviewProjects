namespace DoctorRedis.Domain.Entities
{
    public class Doctor
    {
        public int Id { get; set; }
        public required string FullName { get; set; }
        public required string Specialty { get; set; }
        public ICollection<AppointmentSlot> Slots { get; set; } = new List<AppointmentSlot>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
