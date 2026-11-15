namespace DoctorRedis.Service.DTOs
{
    public class CreateSlotDto
    {
        public int DoctorId { get; set; }
        public DateTime SlotTime { get; set; }
        public bool IsBooked { get; set; }
    }
}
