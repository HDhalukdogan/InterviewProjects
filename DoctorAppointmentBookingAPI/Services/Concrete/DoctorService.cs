using DoctorAppointmentBookingAPI.Data;
using DoctorAppointmentBookingAPI.DTOs;
using DoctorAppointmentBookingAPI.Entities;
using DoctorAppointmentBookingAPI.Services.Abstract;
using Microsoft.EntityFrameworkCore;

namespace DoctorAppointmentBookingAPI.Services.Concrete
{
    public class DoctorService(BookingContext context) : IDoctorService
    {
        public async Task CrerateDoctorAsync(CreateDoctorDto createDoctorDto)
        {
            await context.Doctors.AddAsync(new Doctor
            {
                FullName = createDoctorDto.FullName,
                Specialty = createDoctorDto.Specialty,
                WorkingHoursStart = createDoctorDto.WorkingHoursStart,
                WorkingHoursEnd = createDoctorDto.WorkingHoursEnd,
                SlotDurationMinutes = createDoctorDto.SlotDurationMinutes
            });
            await context.SaveChangesAsync();
        }

        public async Task DeleteDoctorAsync(Guid id)
        {
            var entity = await GetEntityByIdAsync(id);
            if (entity is not null)
            {
                context.Remove(entity);
                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<DoctorDto>> GetAllDoctorsAsync()
        {
            var doctors = await context.Doctors.ToListAsync();
            return doctors.Select(MapToDto);
        }

        public async Task<DoctorDto> GetByIdAsync(Guid id)
        {
            var entity = await GetEntityByIdAsync(id);
            return MapToDto(entity);
        }

        public async Task UpdateDoctorAsync(UpdateDoctorDto updateDoctorDto)
        {
            var entity = await GetEntityByIdAsync(updateDoctorDto.Id);
            entity.FullName = updateDoctorDto.FullName;
            entity.Specialty = updateDoctorDto.Specialty;
            entity.WorkingHoursStart = updateDoctorDto.WorkingHoursStart;
            entity.WorkingHoursEnd = updateDoctorDto.WorkingHoursEnd;
            entity.SlotDurationMinutes = updateDoctorDto.SlotDurationMinutes;
            await context.SaveChangesAsync();
        }


        private Task<Doctor> GetEntityByIdAsync(Guid id) => context.Doctors.SingleOrDefaultAsync(x => x.Id == id);

        private DoctorDto MapToDto(Doctor doctor) => new DoctorDto(doctor.Id, doctor.FullName, doctor.Specialty, doctor.WorkingHoursStart, doctor.WorkingHoursEnd, doctor.SlotDurationMinutes);
    }
}
