using DoctorAppointmentBookingAPI.DTOs;
using DoctorAppointmentBookingAPI.Services.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace DoctorAppointmentBookingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorsController(IDoctorService doctorService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var response = await doctorService.GetAllDoctorsAsync();
            return Ok(response);
        }
        [HttpGet("id")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await doctorService.GetByIdAsync(id);
            return Ok(response);
        }
        [HttpDelete("id")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await doctorService.DeleteDoctorAsync(id);
            return NoContent();
        }
        [HttpPost]
        public async Task<IActionResult> Post(CreateDoctorDto dto)
        {
            await doctorService.CrerateDoctorAsync(dto);
            return NoContent();
        }
        [HttpPut]
        public async Task<IActionResult> Update(UpdateDoctorDto dto)
        {
            await doctorService.UpdateDoctorAsync(dto);
            return NoContent();
        }

    }
}
