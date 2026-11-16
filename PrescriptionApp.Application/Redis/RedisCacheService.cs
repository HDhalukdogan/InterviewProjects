using StackExchange.Redis;
using System.Text.Json;

namespace PrescriptionApp.Application.Redis
{
    public class RedisCacheService(IConnectionMultiplexer redis) : IRedisCacheService
    {
        private readonly IDatabase _db = redis.GetDatabase();

        public async Task<T?> GetAsync<T>(string key)
        {
            var value = await _db.StringGetAsync(key);
            return value.IsNullOrEmpty ? default : JsonSerializer.Deserialize<T>(value.ToString());
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan expiry)
        {
            var json = JsonSerializer.Serialize(value);
            await _db.StringSetAsync(key, json, expiry);
        }

        public async Task RemoveAsync(string key) => await _db.KeyDeleteAsync(key);

        public async Task<bool> AcquireLockAsync(string key, string token, TimeSpan expiry)
        {
            // SET key token NX PX expiry (Atomik kilit alma)
            return await _db.StringSetAsync(key, token, expiry, When.NotExists);
        }

        public async Task ReleaseLockAsync(string key, string token)
        {
            // Sadece kilidi alan owner serbest bırakabilsin (Lua script ile atomik kontrol)
            const string luaScript = """
            if redis.call("get", KEYS[1]) == ARGV[1] then
                return redis.call("del", KEYS[1])
            else
                return 0
            end
            """;

            await _db.ScriptEvaluateAsync(luaScript, [key], [token]);
        }
    }
}
