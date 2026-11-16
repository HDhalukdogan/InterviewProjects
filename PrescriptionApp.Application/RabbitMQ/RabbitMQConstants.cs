namespace PrescriptionApp.Application.RabbitMQ;

public static class RabbitMQConstants
{
    public const string HostName = "localhost";
    public const int Port = 5682;
    public const string UserName = "guest";
    public const string Password = "guest";

    public const string PrescriptionCreatedQueue = "prescription.created.queue";
    public const string PrescriptionDispensedQueue = "prescription.dispensed.queue";
}