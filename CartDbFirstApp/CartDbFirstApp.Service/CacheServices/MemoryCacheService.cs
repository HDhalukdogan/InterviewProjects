using Microsoft.Extensions.Caching.Memory;

namespace CartDbFirstApp.Service.CacheServices
{
    public class MemoryCacheService(IMemoryCache memoryCache) : ICacheService
    {
        public string GenerateKey(params string[] parts)
        {
            return string.Join(":", parts);
        }

        public Task<T?> GetAsync<T>(string key)
        {
            memoryCache.TryGetValue(key, out T? value);
            return Task.FromResult(value);
        }

        public Task RemoveAsync(string key)
        {
            memoryCache.Remove(key);
            return Task.CompletedTask;
        }

        public Task SetAsync<T>(string key, T value, TimeSpan expiry)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiry
            };

            memoryCache.Set(key, value, options);
            return Task.CompletedTask;
        }
    }
}