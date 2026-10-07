namespace DoctorAppointmentBookingAPI.DTOs
{
    public record CreateDoctorDto(string FullName, string Specialty, DateTime WorkingHoursStart, DateTime WorkingHoursEnd, int SlotDurationMinutes);
}
