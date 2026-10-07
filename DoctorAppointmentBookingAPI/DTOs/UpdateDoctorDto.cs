namespace DoctorAppointmentBookingAPI.DTOs
{
    public record UpdateDoctorDto(Guid Id,string FullName, string Specialty, DateTime WorkingHoursStart, DateTime WorkingHoursEnd, int SlotDurationMinutes);
}
