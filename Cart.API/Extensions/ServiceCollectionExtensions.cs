using Cart.Services.Abstracts;
using Cart.Services.Concretes;
using System.Reflection;

namespace Cart.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCrudServices(this IServiceCollection services, Assembly assembly)
        {
            // 1. CrudService<> açık tipini referans alıyoruz
            var openGenericType = typeof(CrudService<,,,,>);

            // 2. Assembly içindeki somut sınıfları filtrele
            var serviceTypes = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && IsSubclassOfRawGeneric(openGenericType, t));

            foreach (var implementationType in serviceTypes)
            {
                // Sınıfın implement ettiği interface'leri al (ör. IUserService)
                var interfaces = implementationType.GetInterfaces()
                    .Where(i => !i.IsGenericType || i.GetGenericTypeDefinition() != typeof(ICrudService<,,,,>));

                foreach (var serviceInterface in interfaces)
                {
                    services.AddScoped(serviceInterface, implementationType);
                }
            }

            return services;
        }

        // Generic miras kontrolü yapan yardımcı metod
        private static bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur) return true;
                toCheck = toCheck.BaseType!;
            }
            return false;
        }
    }
}
