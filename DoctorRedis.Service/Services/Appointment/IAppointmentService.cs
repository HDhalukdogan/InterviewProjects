using DoctorRedis.Service.DTOs;

namespace DoctorRedis.Service.Services
{
    public interface IAppointmentService
    {
        Task<AppointmentDto?> GetAppointmentAsync(int AppointmentId);
        Task<IEnumerable<AppointmentDto>> GetAppointmentsAsync();
        Task<AppointmentDto> CreateAppointmentAsync(CreateAppointmentDto createAppointmentDto);
    }
}
