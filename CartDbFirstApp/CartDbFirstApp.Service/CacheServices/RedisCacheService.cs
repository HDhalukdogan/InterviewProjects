using StackExchange.Redis;
using System.Text.Json;

namespace CartDbFirstApp.Service.CacheServices
{
    public class RedisCacheService(IConnectionMultiplexer connectionMultiplexer) : ICacheService
    {

        private readonly IDatabase _db = connectionMultiplexer.GetDatabase();

        public string GenerateKey(params string[] parts)
        {
            return string.Join(":", parts);
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var jsonString = await _db.StringGetAsync(key);

            if (!jsonString.HasValue)
            {
                return default;
            }

            var value = JsonSerializer.Deserialize<T>(jsonString.ToString());

            return value;
        }

        public async Task RemoveAsync(string key)
        {
            await _db.KeyDeleteAsync(key);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan expiry)
        {

            var jsonString = JsonSerializer.Serialize(value);

            await _db.StringSetAsync(key, jsonString, expiry);
        }
    }
}
