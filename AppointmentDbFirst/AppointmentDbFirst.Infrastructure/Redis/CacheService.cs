using StackExchange.Redis;
using System.Text.Json;

namespace AppointmentDbFirst.Infrastructure.Redis
{
    public class CacheService(IConnectionMultiplexer  redis) : ICacheService
    {
        private readonly IDatabase _db = redis.GetDatabase();
        public string GenerateKey(params string[] parts) => string.Join(":", parts);

        public async Task<T?> GetAsync<T>(string key)
        {
            var value = await _db.StringGetAsync(key);
            if (!value.HasValue) return default;
            return JsonSerializer.Deserialize<T>(value.ToString());
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan expiry)
        {
            var json = JsonSerializer.Serialize(value);
            await _db.StringSetAsync(key, json, expiry);
        }

        public async Task RemoveAsync(string key)
        {
            await _db.KeyDeleteAsync(key);
        }


    }
}
