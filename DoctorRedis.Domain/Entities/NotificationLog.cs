namespace DoctorRedis.Domain.Entities
{
    public class NotificationLog
    {
        public int Id { get; set; }
        public int AppointmentId { get; set; }
        public required string Recipient { get; set; }
        public Appointment Appointment { get; set; } = null!;
    }
}
