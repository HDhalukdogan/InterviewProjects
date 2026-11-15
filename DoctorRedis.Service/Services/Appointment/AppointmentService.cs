using DoctorRedis.Domain.Entities;
using DoctorRedis.Domain.Events;
using DoctorRedis.Infrastructure.RabbitMQ;
using DoctorRedis.Infrastructure.Services;
using DoctorRedis.Persistence.Contexts;
using DoctorRedis.Service.DTOs;
using Microsoft.EntityFrameworkCore;

namespace DoctorRedis.Service.Services
{
    public class AppointmentService(DoctorRedisContext context, ICacheService cacheService, IRabbitMQPublisher rabbitMQPublisher) : IAppointmentService
    {
        public async Task<AppointmentDto> CreateAppointmentAsync(CreateAppointmentDto createAppointmentDto)
        {
            var lockKey = cacheService.GenerateKey("lock", new { slot = createAppointmentDto.SlotId });
            var lockValue = Guid.NewGuid().ToString();
            var isLocked = await cacheService.AcquireLockAsync(lockKey, lockValue, TimeSpan.FromSeconds(10));

            if (!isLocked)
            {
                throw new InvalidOperationException("Bu randevu saati için şu an başka bir işlem yürütülüyor. Lütfen biraz sonra tekrar deneyin.");
            }

            try
            {
                var slot = await context.AppointmentSlots.FirstOrDefaultAsync(s => s.Id == createAppointmentDto.SlotId);
                if (slot is null)
                    throw new KeyNotFoundException("Seçilen randevu slotu bulunamadı.");

                if (slot.IsBooked)
                    throw new InvalidOperationException("Bu randevu saati daha önce rezerve edilmiştir.");

                var patient = await context.Patients.FindAsync(createAppointmentDto.PatientId);
                if (patient is null)
                    throw new KeyNotFoundException("Hasta bulunamadı.");

                var doctor = await context.Doctors.FindAsync(slot.DoctorId);
                if (doctor is null)
                    throw new KeyNotFoundException("Doktor bulunamadı.");

                slot.IsBooked = true;

                var appointment = new Appointment
                {
                    DoctorId = slot.DoctorId,
                    SlotId = slot.Id,
                    PatientId = patient.Id
                };

                await context.Appointments.AddAsync(appointment);
                await context.SaveChangesAsync();

                var cacheKey = cacheService.GenerateKey("slots", new { doctor = doctor.Id, date = slot.SlotTime.ToString("yyyy-MM-dd") });
                await cacheService.RemoveAsync(cacheKey);

                await rabbitMQPublisher.PublishAsync(
                    message: new AppointmentCreatedEvent(
                        appointment.Id,
                        doctor.Id,
                        patient.Id,
                        patient.Email ?? string.Empty,
                        doctor.FullName,
                        slot.SlotTime),
                    exchange: RabbitMQConstants.AppointmentExchange,
                    routingKey: RabbitMQConstants.AppointmentCreatedRoutingKey
                );

                return new AppointmentDto
                {
                    Id = appointment.Id,
                    SlotId = appointment.SlotId,
                };
            }
            finally
            {
                await cacheService.ReleaseLockAsync(lockKey, lockValue);
            }
        }

        public async Task<AppointmentDto?> GetAppointmentAsync(int appointmentId)
        {
            var appointment = await context.Appointments.Where(s => s.Id.Equals(appointmentId)).Select(x => new AppointmentDto
            {
                SlotId = x.SlotId,
                DoctorName = x.Doctor.FullName,
                PatientName = x.Patient.FullName,
            }).FirstOrDefaultAsync();
            return appointment;
        }

        public async Task<IEnumerable<AppointmentDto>> GetAppointmentsAsync()
        {
            return await context.Appointments.Select(x => new AppointmentDto
            {
                SlotId = x.SlotId,
                DoctorName = x.Doctor.FullName,
                PatientName = x.Patient.FullName,
            }).ToListAsync();
        }
    }
}
