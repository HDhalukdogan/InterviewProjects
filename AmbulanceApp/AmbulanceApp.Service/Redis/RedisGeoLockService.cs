using StackExchange.Redis;

namespace AmbulanceApp.Service.Redis;


public class RedisGeoLockService(IConnectionMultiplexer redis) : IRedisGeoLockService
{
    private readonly IDatabase _db = redis.GetDatabase();

    public async Task UpsertAvailableLocationAsync(int ambulanceId, decimal latitude, decimal longitude)
    {
        // Redis GEO parametresi önce Longitude, sonra Latitude alır
        await _db.GeoAddAsync(
            RedisConstants.AvailableAmbulancesGeoKey,
            (double)longitude,
            (double)latitude,
            ambulanceId.ToString());
    }

    public async Task RemoveFromAvailableAsync(int ambulanceId)
    {
        await _db.GeoRemoveAsync(RedisConstants.AvailableAmbulancesGeoKey, ambulanceId.ToString());
    }

    public async Task<List<(int AmbulanceId, double DistanceKm)>> FindNearestAmbulancesAsync(
        decimal latitude, decimal longitude, double radiusKm = 50)
    {
        var results = await _db.GeoRadiusAsync(
            RedisConstants.AvailableAmbulancesGeoKey,
            (double)longitude,
            (double)latitude,
            radiusKm,
            GeoUnit.Kilometers,
            count: 5,
            order: Order.Ascending);

        return results
            .Where(r => int.TryParse(r.Member.ToString(), out _))
            .Select(r => (int.Parse(r.Member.ToString()), r.Distance ?? 0))
            .ToList();
    }

    public async Task<bool> AcquireLockAsync(string key, string token, TimeSpan expiry)
        => await _db.StringSetAsync(key, token, expiry, When.NotExists);

    public async Task ReleaseLockAsync(string key, string token)
    {
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