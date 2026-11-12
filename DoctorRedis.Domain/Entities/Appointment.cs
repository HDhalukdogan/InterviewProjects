namespace DoctorRedis.Domain.Entities
{
    public class Appointment
    {
        public int Id { get; set; }
        public int SlotId { get; set; }
        public int DoctorId { get; set; }
        public int PatientId { get; set; }

        public AppointmentSlot Slot { get; set; } = null!;
        public Doctor Doctor { get; set; } = null!;
        public Patient Patient { get; set; } = null!;
        public ICollection<NotificationLog> NotificationLogs { get; set; } = new List<NotificationLog>();

    }
}
