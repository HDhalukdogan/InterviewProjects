using AutoMapper;
using DoctorRedis.Domain.Entities;
using DoctorRedis.Persistence.Contexts;
using DoctorRedis.Service.DTOs;
using Microsoft.EntityFrameworkCore;

namespace DoctorRedis.Service.Services
{
    public class DoctorService(DoctorRedisContext context, IMapper mapper) : IDoctorService
    {
        public async Task<DoctorDto> CreateDoctorAsync(DoctorCreateDto doctorCreateDto)
        {
            var doctor = new Doctor
            {
                FullName = doctorCreateDto.FullName,
                Specialty = doctorCreateDto.Specialty
            };
            await context.Doctors.AddAsync(doctor);
            await context.SaveChangesAsync();
            return new DoctorDto
            {
                Id = doctor.Id,
                FullName = doctor.FullName,
                Specialty = doctor.Specialty
            };
        }

        public async Task<IEnumerable<DoctorDto>> GetAllDoctorsAsync()
        {
            var doctors = await context.Doctors.Select(d => new DoctorDto
            {
                Id = d.Id,
                FullName = d.FullName,
                Specialty = d.Specialty
            }).ToListAsync();
            return doctors;

        }

        public async Task<DoctorDto> GetDoctorByIdAsync(int id)
        {
            var doctor = await context.Doctors.AsNoTracking().FirstOrDefaultAsync(s=> s.Id.Equals(id));
            if (doctor == null)
                throw new InvalidOperationException("Doctor not found");

            return mapper.Map<DoctorDto>(doctor);
        }
    }
}
