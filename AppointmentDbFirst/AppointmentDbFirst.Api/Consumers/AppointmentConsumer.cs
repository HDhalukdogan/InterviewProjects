using AppointmentDbFirst.Domain.Events;
using AppointmentDbFirst.Infrastructure.RabbitMQ;
using AppointmentDbFirst.Persistence.Contexts;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

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

            await Task.Delay(Timeout.Infinite, stoppingToken);
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


    //public class AppointmentConsumer(IServiceScopeFactory scopeFactory) : BackgroundService
    //{
    //    private IConnection? _connection;
    //    private IChannel? _channel;

    //    public override async Task StartAsync(CancellationToken cancellationToken)
    //    {
    //        var factory = new ConnectionFactory
    //        {
    //            HostName = RabbitMQConstants.HostName,
    //            Port = int.Parse(RabbitMQConstants.Port),
    //            UserName = RabbitMQConstants.UserName,
    //            Password = RabbitMQConstants.Password
    //        };

    //        _connection = await factory.CreateConnectionAsync(cancellationToken);
    //        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

    //        await _channel.QueueDeclareAsync(
    //            queue: RabbitMQConstants.DbFirstDoctorCreatedQueue,
    //            durable: true,
    //            exclusive: false,
    //            autoDelete: false,
    //            arguments: null,
    //            cancellationToken: cancellationToken);

    //        await base.StartAsync(cancellationToken);
    //    }

    //    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    //    {
    //        if (_channel is null) return;

    //        var consumer = new AsyncEventingBasicConsumer(_channel);
    //        consumer.ReceivedAsync += async (_, ea) =>
    //        {
    //            try
    //            {
    //                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
    //                var data = JsonSerializer.Deserialize<AppointmentCreatedEvent>(json);

    //                if (data is not null)
    //                {
    //                    using var scope = scopeFactory.CreateScope();
    //                    var dbContext = scope.ServiceProvider.GetRequiredService<AppointmentDbFirstContext>();

    //                    // Örn: NotificationLog tablosuna kayıt at
    //                    // await dbContext.NotificationLogs.AddAsync(...);
    //                    // await dbContext.SaveChangesAsync(stoppingToken);

    //                    Console.WriteLine(data.Id);
    //                }

    //                await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
    //            }
    //            catch
    //            {
    //                await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true, cancellationToken: stoppingToken);
    //            }
    //        };

    //        await _channel.BasicConsumeAsync(
    //            queue: RabbitMQConstants.DbFirstDoctorCreatedQueue,
    //            autoAck: false,
    //            consumer: consumer,
    //            cancellationToken: stoppingToken);
    //    }

    //    public override async Task StopAsync(CancellationToken cancellationToken)
    //    {
    //        if (_channel is not null)
    //        {
    //            await _channel.CloseAsync(cancellationToken);
    //            await _channel.DisposeAsync();
    //        }

    //        if (_connection is not null)
    //        {
    //            await _connection.CloseAsync(cancellationToken);
    //            await _connection.DisposeAsync();
    //        }

    //        await base.StopAsync(cancellationToken);
    //    }
    //}
}
