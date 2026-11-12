using StackExchange.Redis;
using System.Text.Json;

namespace DoctorRedis.Infrastructure.Services
{
    public class RedisCacheService(IConnectionMultiplexer redis) : ICacheService
    {
        private readonly IDatabase _db = redis.GetDatabase();

        public async Task<bool> AcquireLockAsync(string key, string value, TimeSpan expiry)
        {
            return await _db.LockTakeAsync(key, value, expiry);
        }

        public async Task<bool> ReleaseLockAsync(string key, string value)
        {
            return await _db.LockReleaseAsync(key, value);
        }

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
        public string GenerateKey(params object[] parts)
        {
            if (parts is null || parts.Length == 0)
                throw new ArgumentException("Key oluşturmak için en az bir parametre verilmelidir.", nameof(parts));

            var cleanParts = parts
                .Select(p => p?.ToString()?.Trim().ToLowerInvariant())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToArray();

            if (cleanParts.Length == 0)
                throw new ArgumentException("Geçerli en az bir key parçası verilmelidir.", nameof(parts));

            return string.Join(":", cleanParts);
        }
    }
}
