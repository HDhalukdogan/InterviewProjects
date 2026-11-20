namespace CartDbFirstApp.Service.CacheServices
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan expiry);
        Task RemoveAsync(string key);
        string GenerateKey(params string[] parts);
    }
}
