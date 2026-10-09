using Cart.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cart.Persistence.Repositories
{
    public interface IRepository<TEntity,TKey> where TEntity : BaseEntity<TKey> where TKey : struct
    {
        public DbSet<TEntity> Table { get;}
        Task<int> SaveChangesAsync();
    }
}
