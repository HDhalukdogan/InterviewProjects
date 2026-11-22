public interface IRedisGeoLockService
{
    Task UpsertAvailableLocationAsync(int ambulanceId, decimal latitude, decimal longitude);
    Task RemoveFromAvailableAsync(int ambulanceId);
    Task<List<(int AmbulanceId, double DistanceKm)>> FindNearestAmbulancesAsync(decimal latitude, decimal longitude, double radiusKm = 50);
    Task<bool> AcquireLockAsync(string key, string token, TimeSpan expiry);
    Task ReleaseLockAsync(string key, string token);
}