namespace DoctorRedis.Service.DTOs
{
    public class SlotDto
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public DateTime SlotTime { get; set; }
        public bool IsBooked { get; set; } = false;
    }
}
