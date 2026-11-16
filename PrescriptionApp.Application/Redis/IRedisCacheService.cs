using System;
using System.Collections.Generic;
using System.Text;

namespace PrescriptionApp.Application.Redis
{
    public interface IRedisCacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan expiry);
        Task RemoveAsync(string key);
        Task<bool> AcquireLockAsync(string key, string token, TimeSpan expiry);
        Task ReleaseLockAsync(string key, string token);
    }
}
