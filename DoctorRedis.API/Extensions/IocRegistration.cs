using DoctorRedis.Infrastructure.Services;
using DoctorRedis.Service.Services;

namespace DoctorRedis.API.Extensions
{
    public static class IocRegistration
    {

        public static IServiceCollection AddIocRegistration(this IServiceCollection services)
        {
            services.AddScoped<IDoctorService, DoctorService>();
            services.AddScoped<ICacheService, RedisCacheService>();
            return services;
        }
    }
}
