using Cart.Domain.Entities;
using Cart.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Cart.Persistence.Repositories
{
    public class Repository<TEntity, TKey>(CartContext context) : IRepository<TEntity, TKey> where TEntity : BaseEntity<TKey> where TKey : struct
    {
        public DbSet<TEntity> Table => context.Set<TEntity>();

        public async Task<int> SaveChangesAsync()
        {
            return await context.SaveChangesAsync();
        }
    }
}
