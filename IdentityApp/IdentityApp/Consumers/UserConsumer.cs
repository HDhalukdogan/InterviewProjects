using IdentityApp.Constants;
using IdentityApp.DTOs;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace IdentityApp.Consumers
{

    public class UserConsumer : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            var factory = new ConnectionFactory
            {
                HostName = RabbitMQConstants.HostName,
                Port = int.Parse(RabbitMQConstants.Port),
                UserName = RabbitMQConstants.UserName,
                Password = RabbitMQConstants.Password,
            };
            await using var connection = await factory.CreateConnectionAsync(cancellationToken: stoppingToken);
            await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

            await channel.QueueDeclareAsync(
                queue: RabbitMQConstants.UserQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                cancellationToken: stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (_, ea) =>
            {
                var json = Encoding.UTF8.GetString(ea.Body.Span);
                var dto = JsonSerializer.Deserialize<UserDto>(json);
                Console.WriteLine(dto);
            };

            await channel.BasicConsumeAsync(
                queue: RabbitMQConstants.UserQueue,
                autoAck: true,
                consumer: consumer,
                cancellationToken: stoppingToken);


            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
    //public class UserConsumer : BackgroundService
    //{
    //    private IConnection? connection;
    //    private IChannel? channel;
    //    public override async Task StartAsync(CancellationToken cancellationToken)
    //    {
    //        var factory = new ConnectionFactory
    //        {
    //            HostName = RabbitMQConstants.HostName,
    //            Port = int.Parse(RabbitMQConstants.Port),
    //            UserName = RabbitMQConstants.UserName,
    //            Password = RabbitMQConstants.Password,
    //        };

    //        connection = await factory.CreateConnectionAsync();
    //        channel = await connection.CreateChannelAsync();

    //        await channel.ExchangeDeclareAsync(
    //            exchange: RabbitMQConstants.UserExchange,
    //            type: ExchangeType.Direct,
    //            durable: true,
    //            autoDelete: false,
    //            cancellationToken: cancellationToken
    //            );

    //        await channel.QueueDeclareAsync(
    //            queue: RabbitMQConstants.UserQueue,
    //            durable: true,
    //            exclusive: false,
    //            autoDelete: false,
    //            cancellationToken: default
    //            );

    //        await channel.QueueBindAsync(
    //            queue: RabbitMQConstants.UserQueue,
    //            exchange: RabbitMQConstants.UserExchange,
    //            routingKey: RabbitMQConstants.UserRoutingKey,
    //            cancellationToken: cancellationToken);

    //        await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false, cancellationToken: cancellationToken);


    //        await base.StartAsync(cancellationToken);
    //    }
    //    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    //    {
    //        if (channel is null) return;

    //        var consumer = new AsyncEventingBasicConsumer(channel);

    //        consumer.ReceivedAsync += async (_, ea) =>
    //        {
    //            try
    //            {
    //                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
    //                var dto = JsonSerializer.Deserialize<UserDto>(json);

    //                if (dto is not null)
    //                {
    //                    Console.WriteLine(dto);
    //                }

    //                await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);

    //            }
    //            catch (Exception)
    //            {

    //                await channel.BasicAckAsync(deliveryTag:ea.DeliveryTag,multiple: false, cancellationToken: stoppingToken);
    //            }
    //        };

    //        await channel.BasicConsumeAsync(
    //           queue: RabbitMQConstants.UserQueue,
    //           autoAck: false,
    //           consumer: consumer,
    //           cancellationToken: stoppingToken
    //            );
    //        await Task.Delay(Timeout.Infinite, stoppingToken);
    //    }

    //    public override async Task StopAsync(CancellationToken cancellationToken)
    //    {

    //        if (channel is not null)
    //        {
    //            await channel.CloseAsync(cancellationToken);
    //            await channel.DisposeAsync();
    //        }

    //        if (connection is not null)
    //        {
    //            await connection.CloseAsync(cancellationToken);
    //            await connection.DisposeAsync();
    //        }

    //        await base.StopAsync(cancellationToken);
    //    }
    //}
}
