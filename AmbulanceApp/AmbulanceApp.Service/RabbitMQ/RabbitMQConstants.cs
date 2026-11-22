namespace AmbulanceApp.Service.RabbitMQ;

public static class RabbitMQConstants
{
    public const string HostName = "localhost";
    public const int Port = 5682;
    public const string UserName = "guest";
    public const string Password = "guest";

    public const string EmergencyCallCreatedQueue = "ambulance.emergency-call.created.queue";
    public const string AmbulanceDispatchedQueue = "ambulance.dispatched.queue";
    public const string DispatchStatusChangedQueue = "ambulance.dispatch.status-changed.queue";
}