using DoctorAppointmentBookingAPI.Data;
using DoctorAppointmentBookingAPI.DTOs;
using DoctorAppointmentBookingAPI.Entities;
using DoctorAppointmentBookingAPI.Services.Abstract;
using Microsoft.EntityFrameworkCore;

namespace DoctorAppointmentBookingAPI.Services.Concrete
{
    public class AppointmentService(BookingContext context) : IAppointmentService
    {
        public async Task<IEnumerable<AppointmentDto>> GetAllAppointmentsAsync()
        {
            var appointments = await context.Appointments.Select(s => new AppointmentDto(s.Doctor.FullName, s.Patient.FullName, s.StartTime, s.EndTime, s.Status)).ToListAsync();
            return appointments;
        }
        public async Task CreateAppointmentAsync(CreateAppointmentDto createAppointmentDto)
        {
            var patient = await context.Patients.FirstOrDefaultAsync(s => s.FullName.Equals(createAppointmentDto.FullName));
            if (patient is null)
            {
                patient = new Patient
                {
                    FullName = createAppointmentDto.FullName,
                    Phone = createAppointmentDto.Phone,
                    Email = createAppointmentDto.Email
                };
                await context.Patients.AddAsync(patient);
                await context.SaveChangesAsync();
            }

            var utcStartTime = createAppointmentDto.StartTime.Kind == DateTimeKind.Utc
                            ? createAppointmentDto.StartTime
                            : DateTime.SpecifyKind(createAppointmentDto.StartTime, DateTimeKind.Utc);

            var utcEndTime = createAppointmentDto.EndTime.Kind == DateTimeKind.Utc
                ? createAppointmentDto.EndTime
                : DateTime.SpecifyKind(createAppointmentDto.EndTime, DateTimeKind.Utc);

            var hasDoctorAppointment = await context.Appointments.AnyAsync(s => s.DoctorId.Equals(createAppointmentDto.DoctorId)
            && ((s.StartTime <= utcStartTime && s.EndTime >= utcStartTime) || (s.StartTime <= utcEndTime && s.EndTime >= utcEndTime)));

            if (hasDoctorAppointment)
                throw new Exception("Doctor has an appointment at this time!");

            var appointment = new Appointment
            {
                DoctorId = createAppointmentDto.DoctorId,
                PatientId = patient.Id,
                StartTime = utcStartTime,
                EndTime = utcEndTime,
            };
            await context.Appointments.AddAsync(appointment);
            await context.SaveChangesAsync();
        }


    }
}
