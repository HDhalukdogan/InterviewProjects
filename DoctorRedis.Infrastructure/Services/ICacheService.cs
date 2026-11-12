namespace DoctorRedis.Infrastructure.Services
{
    public interface ICacheService
    {
        Task<bool> AcquireLockAsync(string key, string value, TimeSpan expiry);
        Task<bool> ReleaseLockAsync(string key, string value);
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan expiry);
        Task RemoveAsync(string key);
        string GenerateKey(params object[] parts);
    }
}
