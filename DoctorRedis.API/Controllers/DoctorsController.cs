using DoctorRedis.Service.DTOs;
using DoctorRedis.Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DoctorRedis.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorsController(IDoctorService doctorService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllDoctors()
        {
            var doctors = await doctorService.GetAllDoctorsAsync();
            return Ok(doctors);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDoctorById(int id)
        {
            var doctor = await doctorService.GetDoctorByIdAsync(id);
            return Ok(doctor);
        }
        [HttpPost]
        public async Task<IActionResult> CreateDoctor([FromBody] DoctorCreateDto doctorCreateDto)
        {
            var doctor = await doctorService.CreateDoctorAsync(doctorCreateDto);
            return Ok(doctor);
        }
    }
}
