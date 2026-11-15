namespace DoctorRedis.Service.DTOs
{
    public class CreateAppointmentDto
    {
        public int SlotId { get; set; }
        public int DoctorId { get; set; }
        public int PatientId { get; set; }
    }
}
