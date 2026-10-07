namespace DoctorAppointmentBookingAPI.DTOs
{
    public record CreateAppointmentDto(string FullName, string Email, string Phone, Guid DoctorId, DateTime StartTime, DateTime EndTime);
}
