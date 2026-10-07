using Cart.Domain.Entities;

namespace Cart.Services.Abstracts
{
    public interface ICrudService<T> where T : BaseEntity<Guid>
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task CreateAsync(T entity, bool autoSave = true);
        Task<int> CompleteAsync();
    }
}
