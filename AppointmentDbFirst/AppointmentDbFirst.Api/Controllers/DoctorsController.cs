using AppointmentDbFirst.Service.DTOs.Doctor;
using AppointmentDbFirst.Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentDbFirst.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorsController(IDoctorService doctorService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var doctors = await doctorService.GetAllAsync();
            return Ok(doctors);
        }

        [HttpPost]
        public async Task<IActionResult> Create(DoctorCreateDto doctorCreateDto)
        {
            var doctor = await doctorService.CreateAsync(doctorCreateDto);
            return Ok(doctor);
        }
    }
}
