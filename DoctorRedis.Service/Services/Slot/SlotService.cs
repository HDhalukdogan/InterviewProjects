using DoctorRedis.Domain.Entities;
using DoctorRedis.Infrastructure.Services;
using DoctorRedis.Persistence.Contexts;
using DoctorRedis.Service.DTOs;
using Microsoft.EntityFrameworkCore;

namespace DoctorRedis.Service.Services
{
    public class SlotService(
        DoctorRedisContext context,
        ICacheService cacheService) : ISlotService
    {
        public async Task<SlotDto> CreateSlotAsync(CreateSlotDto createSlotDto)
        {
            var slot = new AppointmentSlot
            {
                DoctorId = createSlotDto.DoctorId,
                IsBooked = createSlotDto.IsBooked,
                SlotTime = createSlotDto.SlotTime,
            };

            await context.AppointmentSlots.AddAsync(slot);
            await context.SaveChangesAsync();

            var cacheKey = cacheService.GenerateKey("slots", new { doctor = slot.DoctorId, date = slot.SlotTime.ToString("yyyy-MM-dd") });
            await cacheService.RemoveAsync(cacheKey);

            return new SlotDto
            {
                Id = slot.Id,
                DoctorId = slot.DoctorId,
                IsBooked = slot.IsBooked,
                SlotTime = slot.SlotTime
            };
        }

        public async Task<List<SlotDto>> GetAvailableSlotsByDoctorAndDateAsync(int doctorId, DateTime date)
        {
            var cacheKey = cacheService.GenerateKey("slots", new { doctor = doctorId, date = date.ToString("yyyy-MM-dd") });

            var cachedSlots = await cacheService.GetAsync<List<SlotDto>>(cacheKey);
            if (cachedSlots is not null)
            {
                return cachedSlots;
            }

            var startOfDay = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);
            var endOfDay = startOfDay.AddDays(1);

            var availableSlots = await context.AppointmentSlots
                .AsNoTracking()
                .Where(s => s.DoctorId == doctorId
                         && !s.IsBooked
                         && s.SlotTime >= startOfDay
                         && s.SlotTime < endOfDay)
                .OrderBy(s => s.SlotTime)
                .Select(slot => new SlotDto
                {
                    Id = slot.Id,
                    DoctorId = slot.DoctorId,
                    IsBooked = slot.IsBooked,
                    SlotTime = slot.SlotTime
                })
                .ToListAsync();

            await cacheService.SetAsync(cacheKey, availableSlots, TimeSpan.FromMinutes(10));

            return availableSlots;
        }

        public async Task<SlotDto> GetSlotAsync(int slotId)
        {
            var slot = await context.AppointmentSlots.AsNoTracking().FirstOrDefaultAsync(s => s.Id == slotId);
            if (slot is null)
                throw new InvalidOperationException("Slot not found");

            return new SlotDto
            {
                Id = slot.Id,
                DoctorId = slot.DoctorId,
                IsBooked = slot.IsBooked,
                SlotTime = slot.SlotTime
            };
        }

        public async Task<IEnumerable<SlotDto>> GetSlotsAsync()
        {
            return await context.AppointmentSlots
                .AsNoTracking()
                .Select(slot => new SlotDto
                {
                    Id = slot.Id,
                    DoctorId = slot.DoctorId,
                    IsBooked = slot.IsBooked,
                    SlotTime = slot.SlotTime
                })
                .ToListAsync();
        }
    }
}
