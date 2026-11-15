using AppointmentDbFirst.Service.DTOs;

namespace AppointmentDbFirst.Service.Services
{
    public interface IDoctorService
    {
        Task<IEnumerable<DoctorDto>> GetAllAsync();
    }
}
