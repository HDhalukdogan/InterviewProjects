using DoctorAppointmentBookingAPI.DTOs;

namespace DoctorAppointmentBookingAPI.Services.Abstract
{
    public interface IAppointmentService
    {
        public Task<IEnumerable<AppointmentDto>> GetAllAppointmentsAsync();
        public Task CreateAppointmentAsync(CreateAppointmentDto createAppointmentDto);
    }
}
