using DoctorRedis.API.Consumers;
using DoctorRedis.Infrastructure.RabbitMQ;
using DoctorRedis.Infrastructure.Services;
using DoctorRedis.Persistence.Contexts;
using DoctorRedis.Service.DTOs;
using DoctorRedis.Service.Mappings;
using DoctorRedis.Service.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace DoctorRedis.API.Extensions
{
    public static class IocRegistration
    {

        public static IServiceCollection AddIocRegistration(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddDbContext<DoctorRedisContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DoctorRedisApp") ?? throw new InvalidOperationException("Connection string 'DoctorRedisApp' not found.")));

            services.AddSingleton<IConnectionMultiplexer>(_ =>
                ConnectionMultiplexer.Connect(configuration.GetConnectionString("RedisDb") ?? throw new InvalidOperationException("Connection string 'RedisDb' not found.")));

            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<DoctorCreateDtoValidator>();
            services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());
            //builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile).Assembly);
            services.AddHostedService<AppointmentCreatedConsumer>();


            services.AddScoped<IDoctorService, DoctorService>();
            services.AddScoped<IPatientService, PatientService>();
            services.AddScoped<ISlotService, SlotService>();
            services.AddScoped<IAppointmentService, AppointmentService>();


            services.AddScoped<ICacheService, RedisCacheService>();
            services.AddSingleton<IRabbitMQConnection, RabbitMQConnection>();
            services.AddScoped<IRabbitMQPublisher, RabbitMQPublisher>();
            return services;
        }
    }
}
