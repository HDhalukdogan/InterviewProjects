namespace DoctorRedis.Domain.Events
{
    public record AppointmentCreatedEvent(
        int AppointmentId,
        int DoctorId,
        int PatientId,
        string PatientEmail,
        string DoctorName,
        DateTime SlotTime
    );
}
