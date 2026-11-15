namespace DoctorRedis.Infrastructure.RabbitMQ
{
    public static class RabbitMQConstants
    {
        public const string AppointmentExchange = "appointment.exchange";
        public const string AppointmentCreatedQueue = "appointment.created.queue";
        public const string AppointmentCreatedRoutingKey = "appointment.created";
    }
}
