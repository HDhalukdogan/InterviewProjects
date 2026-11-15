using DoctorRedis.Service.DTOs;
using DoctorRedis.Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace DoctorRedis.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SlotsController(ISlotService slotService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAlSlots()
        {
            var slots = await slotService.GetSlotsAsync();
            return Ok(slots);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSlotById(int id)
        {
            var slot = await slotService.GetSlotAsync(id);
            return Ok(slot);
        }
        [HttpPost]
        public async Task<IActionResult> CreateSlot([FromBody]CreateSlotDto createSlotDto)
        {
            var slot = await slotService.CreateSlotAsync(createSlotDto);
            return Ok(slot);
        }
    }
}
