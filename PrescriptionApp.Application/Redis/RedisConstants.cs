namespace PrescriptionApp.Application.Redis;

public static class RedisConstants
{
    public const string ConnectionString = "localhost:6389";
    public static string PrescriptionCacheKey(string code) => $"prescription:code:{code}";
    public static string DispenseLockKey(string code) => $"lock:prescription:dispense:{code}";
}