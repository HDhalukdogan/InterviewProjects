using DoctorRedis.Service.DTOs;
namespace DoctorRedis.Service.Services
{
    public interface IDoctorService
    {
        Task<DoctorDto> CreateDoctorAsync(DoctorCreateDto doctorCreateDto);
        Task<IEnumerable<DoctorDto>> GetAllDoctorsAsync();
        Task<DoctorDto> GetDoctorByIdAsync(int id);
    }
}
