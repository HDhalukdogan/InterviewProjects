using DoctorRedis.Service.DTOs;
using DoctorRedis.Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace DoctorRedis.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController(IAppointmentService AppointmentService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAlAppointments()
        {
            var Appointments = await AppointmentService.GetAppointmentsAsync();
            return Ok(Appointments);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAppointmentById(int id)
        {
            var Appointment = await AppointmentService.GetAppointmentAsync(id);
            return Ok(Appointment);
        }
        [HttpPost]
        public async Task<IActionResult> CreateAppointment([FromBody]CreateAppointmentDto createAppointmentDto)
        {
            var Appointment = await AppointmentService.CreateAppointmentAsync(createAppointmentDto);
            return Ok(Appointment);
        }
    }
}
