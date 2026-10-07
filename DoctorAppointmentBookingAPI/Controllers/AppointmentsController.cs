using DoctorAppointmentBookingAPI.DTOs;
using DoctorAppointmentBookingAPI.Services.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace DoctorAppointmentBookingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController(IAppointmentService appointmentService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var response = await appointmentService.GetAllAppointmentsAsync();
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Post(CreateAppointmentDto createAppointmentDto)
        {
            await appointmentService.CreateAppointmentAsync(createAppointmentDto);
            return NoContent();
        }
    }
}
