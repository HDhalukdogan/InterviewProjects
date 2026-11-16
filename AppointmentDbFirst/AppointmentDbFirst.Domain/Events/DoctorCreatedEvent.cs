namespace AppointmentDbFirst.Domain.Events
{
    public record DoctorCreatedEvent(Guid Id, string FullName);
}
