using DoctorAppointmentBookingAPI.DTOs;

namespace DoctorAppointmentBookingAPI.Services.Abstract
{
    public interface IDoctorService
    {
        public Task<IEnumerable<DoctorDto>> GetAllDoctorsAsync();
        public Task<DoctorDto> GetByIdAsync(Guid id);
        public Task CrerateDoctorAsync(CreateDoctorDto createDoctorDto);
        public Task UpdateDoctorAsync(UpdateDoctorDto updateDoctorDto);
        public Task DeleteDoctorAsync(Guid id);
    }
}
