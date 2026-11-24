using IdentityApp.Constants;
using IdentityApp.DTOs;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using StackExchange.Redis;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace IdentityApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherForecastController(IConnectionMultiplexer redis) : ControllerBase
    {
        private readonly IDatabase _db = redis.GetDatabase();
        private static readonly string[] Summaries =
        [
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        ];

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            var username = User.Identity?.Name ?? "Anonymous";
            var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            var guid = Guid.TryParse(userId, out var parsedGuid) ? parsedGuid : Guid.Empty;

            var userDto = new UserDto(guid, username);
            var json = JsonSerializer.Serialize(userDto);

            try
            {
                await _db.StringSetAsync("user", json, TimeSpan.FromSeconds(5));
                var jsonString = await _db.StringGetAsync("user");
                if (!jsonString.IsNullOrEmpty)
                {
                    var obj = JsonSerializer.Deserialize<UserDto>(jsonString.ToString()!);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Redis error in WeatherForecast: {ex.Message}");
            }

            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = RabbitMQConstants.HostName,
                    Port = int.Parse(RabbitMQConstants.Port),
                    UserName = RabbitMQConstants.UserName,
                    Password = RabbitMQConstants.Password,
                };
                await using var connection = await factory.CreateConnectionAsync();
                await using var channel = await connection.CreateChannelAsync();

                var body = Encoding.UTF8.GetBytes(json);

                await channel.BasicPublishAsync(
                    exchange: string.Empty,
                    routingKey: RabbitMQConstants.UserQueue,
                    body: body);

            //var properties = new BasicProperties
            //{
            //    Persistent = true,
            //    ContentType = "application/json"
            //};

            //await channel.BasicPublishAsync(
            //    exchange: RabbitMQConstants.UserExchange,
            //    routingKey: RabbitMQConstants.UserRoutingKey,
            //    mandatory: false,
            //    basicProperties: properties,
            //    body: body,
            //    cancellationToken: default);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RabbitMQ error in WeatherForecast: {ex.Message}");
            }

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
