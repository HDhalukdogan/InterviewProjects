using CartDbFirstApp.Persistence.Contexts;
using CartDbFirstApp.Service.CacheServices;
using CartDbFirstApp.Service.DTOs;
using CartDbFirstApp.Service.RabbitMQ;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace CartDbFirstApp.Service.Services
{
    public class CartService(CartDbFirstAppContext context, [FromKeyedServices("memory")] ICacheService cacheService) : ICartService
    {
        public async Task<IEnumerable<CartDto>> GetAllAsync()
        {
            var key = cacheService.GenerateKey("cart", "all");

            var carts = await cacheService.GetAsync<IEnumerable<CartDto>>(key);

            if (carts is null)
            {
                carts = await context.Carts.Select(c => new CartDto
                {
                    Id = c.Id,
                    UserId = c.UserId,
                    AppliedCouponCode = c.AppliedCouponCode
                }).ToListAsync();

                await cacheService.SetAsync(key, carts, TimeSpan.FromSeconds(30));
            }

            var factory = new ConnectionFactory
            {
                HostName = RabbitMQConstants.RabbitMQHostName,
                Port = int.Parse(RabbitMQConstants.RabbitMQPort),
                UserName = RabbitMQConstants.RabbitMQUserName,
                Password = RabbitMQConstants.RabbitMQPassword,
            };

            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            var json = JsonSerializer.Serialize(carts);

            var body = Encoding.UTF8.GetBytes(json);

            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: RabbitMQConstants.RabbitMQCartQueue,
                body: body
                );


            return carts;
        }
    }
}
