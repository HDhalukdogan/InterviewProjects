using DoctorAppointmentBookingAPI.Entities;

namespace DoctorAppointmentBookingAPI.DTOs
{
    public record AppointmentDto(string DoctorName, string PatientName, DateTime StartTime, DateTime EndTime, Status Status);
}
