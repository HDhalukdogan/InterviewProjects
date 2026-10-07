using Cart.Domain.Entities;
using Cart.Persistence.Repositories;
using Cart.Services.Abstracts;
using Microsoft.EntityFrameworkCore;

namespace Cart.Services.Concretes
{
    public class CrudService<T>(IRepository<T> repo) : ICrudService<T> where T : BaseEntity<Guid>
    {
        protected DbSet<T> Repository => repo.Table;

        public async Task CreateAsync(T entity, bool autoSave = true)
        {
            await Repository.AddAsync(entity);
            if (autoSave)
            {
                await CompleteAsync();
            }
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await Repository.ToListAsync();
        }

        public Task<int> CompleteAsync()
        {
            return repo.SaveChangesAsync();
        }
    }
}
