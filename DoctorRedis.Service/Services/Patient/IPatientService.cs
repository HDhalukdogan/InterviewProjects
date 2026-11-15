using DoctorRedis.Service.DTOs;
namespace DoctorRedis.Service.Services
{
    public interface IPatientService
    {
        Task<PatientDto> CreatePatientAsync(PatientCreateDto patientCreateDto);
        Task<IEnumerable<PatientDto>> GetAllPatientsAsync();
        Task<PatientDto> GetPatientByIdAsync(int id);
    }
}
