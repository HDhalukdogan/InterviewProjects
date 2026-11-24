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

        [HttpGet("user-info")]
        public IActionResult GetUserInfo()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return Ok(new
                {
                    isAuthenticated = true,
                    email = User.Identity.Name,
                    userId = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                });
            }
            return Ok(new { isAuthenticated = false, message = "Çerez bulunamadı veya anonim kullanıcı." });
        }

        [HttpPost]
        public async Task<IActionResult> Post(CreateAppointmentDto createAppointmentDto)
        {
            await appointmentService.CreateAppointmentAsync(createAppointmentDto);
            return NoContent();
        }
    }
}
