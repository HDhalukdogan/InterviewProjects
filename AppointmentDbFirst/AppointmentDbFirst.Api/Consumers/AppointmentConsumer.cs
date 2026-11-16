using AppointmentDbFirst.Domain.Events;
using AppointmentDbFirst.Infrastructure.RabbitMQ;

namespace AppointmentDbFirst.Api.Consumers
{
    public class AppointmentConsumer(
    IRabbitMQService rabbitMQService,
    IServiceScopeFactory scopeFactory) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await ConsumeAppointmentCreated(stoppingToken);
            await ConsumeDoctorCreated(stoppingToken);
        }

        private async Task ConsumeAppointmentCreated(CancellationToken stoppingToken)
        {
            await rabbitMQService.ConsumeAsync<AppointmentCreatedEvent>(
                RabbitMQConstants.DbFirstAppointmentCreatedQueue,
                async (message) =>
                {
                    // DbContext veya Scoped servis kullanacaksan mutlaka yeni scope açmalısın
                    using var scope = scopeFactory.CreateScope();
                    // var logService = scope.ServiceProvider.GetRequiredService<INotificationLogService>();

                    Console.WriteLine($"Yeni randevu eventi işlendi: {message.Id}");
                    await Task.CompletedTask;
                },
                stoppingToken);
        }

        private async Task ConsumeDoctorCreated(CancellationToken stoppingToken)
        {
            await rabbitMQService.ConsumeAsync<DoctorCreatedEvent>(
                RabbitMQConstants.DbFirstDoctorCreatedQueue,
                async (message) =>
                {
                    // DbContext veya Scoped servis kullanacaksan mutlaka yeni scope açmalısın
                    using var scope = scopeFactory.CreateScope();
                    // var logService = scope.ServiceProvider.GetRequiredService<INotificationLogService>();

                    Console.WriteLine($"Yeni doktor eventi işlendi: {message.Id}");
                    await Task.CompletedTask;
                },
                stoppingToken);
        }
    }
}
