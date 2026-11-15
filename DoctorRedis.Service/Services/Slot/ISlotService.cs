using DoctorRedis.Service.DTOs;

namespace DoctorRedis.Service.Services
{
    public interface ISlotService
    {
        Task<SlotDto> GetSlotAsync(int slotId);
        Task<IEnumerable<SlotDto>> GetSlotsAsync();
        Task<SlotDto> CreateSlotAsync(CreateSlotDto createSlotDto);
    }
}
