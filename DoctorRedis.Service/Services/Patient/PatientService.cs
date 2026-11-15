using AutoMapper;
using DoctorRedis.Domain.Entities;
using DoctorRedis.Persistence.Contexts;
using DoctorRedis.Service.DTOs;
using Microsoft.EntityFrameworkCore;

namespace DoctorRedis.Service.Services
{
    public class PatientService(DoctorRedisContext context, IMapper mapper) : IPatientService
    {
        public async Task<PatientDto> CreatePatientAsync(PatientCreateDto patientCreateDto)
        {
            var patient = new Patient
            {
                FullName = patientCreateDto.FullName,
                BirthDate = patientCreateDto.BirthDate,
                Email = patientCreateDto.Email,
                IdentityNumber = patientCreateDto.IdentityNumber,
            };
            await context.Patients.AddAsync(patient);
            await context.SaveChangesAsync();
            return new PatientDto
            {
                Id = patient.Id,
                FullName = patient.FullName,
                BirthDate = patient.BirthDate,
                Email = patient.Email,
                IdentityNumber = patient.IdentityNumber,
            };
        }

        public async Task<IEnumerable<PatientDto>> GetAllPatientsAsync()
        {
            var patients = await context.Patients.Select(d => new PatientDto
            {
                Id = d.Id,
                FullName = d.FullName,
                BirthDate = d.BirthDate,
                Email = d.Email,
                IdentityNumber = d.IdentityNumber,
            }).ToListAsync();
            return patients;

        }

        public async Task<PatientDto> GetPatientByIdAsync(int id)
        {
            var Patient = await context.Patients.AsNoTracking().FirstOrDefaultAsync(s=> s.Id.Equals(id));
            if (Patient == null)
                throw new InvalidOperationException("Patient not found");

            return mapper.Map<PatientDto>(Patient);
        }
    }
}
