using DoctorRedis.Service.DTOs;
using DoctorRedis.Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace DoctorRedis.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController(IPatientService patientService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllPatients()
        {
            var patients = await patientService.GetAllPatientsAsync();
            return Ok(patients);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPatientById(int id)
        {
            var patient = await patientService.GetPatientByIdAsync(id);
            return Ok(patient);
        }
        [HttpPost]
        public async Task<IActionResult> CreatePatient([FromBody] PatientCreateDto patientCreateDto)
        {
            var patient = await patientService.CreatePatientAsync(patientCreateDto);
            return Ok(patient);
        }
    }
}
