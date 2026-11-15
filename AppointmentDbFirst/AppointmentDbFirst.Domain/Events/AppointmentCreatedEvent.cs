namespace AppointmentDbFirst.Domain.Events
{
    public record AppointmentCreatedEvent(Guid Id, Guid DoctorId, Guid PatientId);
}
