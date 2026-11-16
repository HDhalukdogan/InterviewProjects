using AppointmentDbFirst.Service.DTOs;
using AppointmentDbFirst.Service.DTOs.Doctor;

namespace AppointmentDbFirst.Service.Services
{
    public interface IDoctorService
    {
        Task<IEnumerable<DoctorDto>> GetAllAsync();
        Task<DoctorDto> CreateAsync(DoctorCreateDto doctorCreateDto);
    }
}
