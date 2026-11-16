using AppointmentDbFirst.Domain.Entities;
using AppointmentDbFirst.Domain.Events;
using AppointmentDbFirst.Infrastructure.RabbitMQ;
using AppointmentDbFirst.Infrastructure.Redis;
using AppointmentDbFirst.Persistence.Contexts;
using AppointmentDbFirst.Service.DTOs;
using AppointmentDbFirst.Service.DTOs.Doctor;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

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
            await context.Doctors.AddAsync(entity);

            await context.SaveChangesAsync();


            await rabbitMQService.PublishAsync(RabbitMQConstants.DbFirstDoctorCreatedQueue, new DoctorCreatedEvent(entity.Id,entity.FullName));

            //var factory = new ConnectionFactory
            //{
            //    HostName = RabbitMQConstants.HostName,
            //    Port = int.Parse(RabbitMQConstants.Port),
            //    UserName = RabbitMQConstants.UserName,
            //    Password = RabbitMQConstants.Password
            //};

            //await using var connection = await factory.CreateConnectionAsync();
            //await using var channel = await connection.CreateChannelAsync();

            //await channel.QueueDeclareAsync(
            //queue: RabbitMQConstants.DbFirstDoctorCreatedQueue,
            //durable: true,
            //exclusive: false,
            //autoDelete: false,
            //arguments: null);

            //var eventMessage = new DoctorCreatedEvent(entity.Id, entity.FullName);

            //var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(eventMessage));
            //var properties = new BasicProperties { Persistent = true };

            //await channel.BasicPublishAsync(
            //    exchange: string.Empty,
            //    routingKey: RabbitMQConstants.DbFirstDoctorCreatedQueue,
            //    mandatory: false,
            //    basicProperties: properties,
            //    body: body);


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
