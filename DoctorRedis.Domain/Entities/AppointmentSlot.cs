namespace DoctorRedis.Domain.Entities
{
    public class AppointmentSlot
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public DateTime SlotTime { get; set; }
        public bool IsBooked { get; set; } = false;
        public Doctor Doctor { get; set; } = null!;
    }
}
