using Cart.Domain.Entities;
using Cart.Services.DTOs;

namespace Cart.Services.Abstracts
{
    public interface ICrudService<TEntity, TDto, TCreateDto, TUpdateDto, TKey> where TEntity : BaseEntity<TKey> where TDto:BaseDto<TKey> where TUpdateDto : BaseDto<TKey> where TKey : struct
    {
        Task<IEnumerable<TDto>> GetAllAsync();
        Task<TDto?> GetByIdAsync(TKey id);
        Task<TDto> CreateAsync(TCreateDto entity, bool autoSave = true);
        Task<TDto> UpdateAsync(TUpdateDto entity, bool autoSave = true);
        Task<bool> DeleteAsync(TKey id, bool autoSave = true);
        Task<int> CompleteAsync();
    }
}
