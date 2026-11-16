using AppointmentDbFirst.Domain.Entities;
using AppointmentDbFirst.Domain.Events;
using AppointmentDbFirst.Infrastructure.RabbitMQ;
using AppointmentDbFirst.Infrastructure.Redis;
using AppointmentDbFirst.Persistence.Contexts;
using AppointmentDbFirst.Service.DTOs;
using AppointmentDbFirst.Service.DTOs.Doctor;
using Microsoft.EntityFrameworkCore;

namespace AppointmentDbFirst.Service.Services
{
    public class DoctorService(AppointmentDbFirstContext context, ICacheService cacheService, IRabbitMQService rabbitMQService) : IDoctorService
    {
        public async Task<DoctorDto> CreateAsync(DoctorCreateDto doctorCreateDto)
        {
            var entity = new Doctor
            {
                Id = Guid.NewGuid(),
                FullName = doctorCreateDto.FullName,
                Specialty = doctorCreateDto.Specialty,
                SlotDurationMinutes = doctorCreateDto.SlotDurationMinutes,
                WorkingHoursStart = doctorCreateDto.WorkingHoursStart,
                WorkingHoursEnd = doctorCreateDto.WorkingHoursEnd,
            };
            var doctor = await context.Doctors.AddAsync(entity);

            await context.SaveChangesAsync();


            await rabbitMQService.PublishAsync(RabbitMQConstants.DbFirstDoctorCreatedQueue, new DoctorCreatedEvent(entity.Id,entity.FullName));

            var key = cacheService.GenerateKey("allDoctors", "noFilter");

            await cacheService.RemoveAsync(key);

            return new DoctorDto
            {
                Id = entity.Id,
                FullName = entity.FullName,
                Specialty = entity.Specialty,
                SlotDurationMinutes = entity.SlotDurationMinutes,
                WorkingHoursStart = entity.WorkingHoursStart,
                WorkingHoursEnd = entity.WorkingHoursEnd,
            };
        }

        public async Task<IEnumerable<DoctorDto>> GetAllAsync()
        {
            var key = cacheService.GenerateKey("allDoctors", "noFilter");

            var doctors = await cacheService.GetAsync<IEnumerable<DoctorDto>>(key);

            if (doctors is not null)
            {
                return doctors;
            }


            doctors = await context.Doctors.AsNoTracking().Select(d => new DoctorDto
            {
                Id = d.Id,
                FullName = d.FullName,
                Specialty = d.Specialty,
                WorkingHoursStart = d.WorkingHoursStart,
                WorkingHoursEnd = d.WorkingHoursEnd,
                SlotDurationMinutes = d.SlotDurationMinutes
            }).ToListAsync();

            await cacheService.SetAsync<IEnumerable<DoctorDto>>(key, doctors, TimeSpan.FromSeconds(30));

            return doctors;
        }
    }
}
