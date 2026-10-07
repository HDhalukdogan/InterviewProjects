namespace DoctorAppointmentBookingAPI.DTOs
{
    public record DoctorDto(Guid Id, string FullName, string Specialty, DateTime WorkingHoursStart, DateTime WorkingHoursEnd, int SlotDurationMinutes);
}
