using AppointmentDbFirst.Persistence.Contexts;
using AppointmentDbFirst.Service.DTOs;
using Microsoft.EntityFrameworkCore;

namespace AppointmentDbFirst.Service.Services
{
    public class DoctorService(AppointmentDbFirstContext context) : IDoctorService
    {
        public async Task<IEnumerable<DoctorDto>> GetAllAsync()
        {
            var doctors = await context.Doctors.AsNoTracking().Select(d => new DoctorDto
            {
                Id = d.Id,
                FullName = d.FullName,
                Specialty = d.Specialty,
                WorkingHoursStart = d.WorkingHoursStart,
                WorkingHoursEnd = d.WorkingHoursEnd,
                SlotDurationMinutes = d.SlotDurationMinutes
            }).ToListAsync();

            return doctors;
        }
    }
}
