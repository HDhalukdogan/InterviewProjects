namespace AmbulanceApp.Service.Redis
{
    public static class RedisConstants
    {
        public const string ConnectionString = "localhost:6389";
        public const string AvailableAmbulancesGeoKey = "ambulances:available:geo";
        public static string AmbulanceLockKey(int ambulanceId) => $"lock:ambulance:dispatch:{ambulanceId}";
    }
}
