namespace AppointmentDbFirst.Infrastructure.RabbitMQ
{
    public static class RabbitMQConstants
    {
        public const string HostName = "localhost";
        public const string Port = "5682";
        public const string UserName = "guest";
        public const string Password = "guest";
        public const string DbFirstAppointmentCreatedQueue = "dbfirst.appointment.created.queue";
        public const string DbFirstDoctorCreatedQueue = "dbfirst.doctor.created.queue";
    }
}
