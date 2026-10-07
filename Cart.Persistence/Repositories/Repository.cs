using Cart.Domain.Entities;
using Cart.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Cart.Persistence.Repositories
{
    public class Repository<T>(CartContext context) : IRepository<T> where T : BaseEntity<Guid>
    {
        public DbSet<T> Table => context.Set<T>();

        public async Task<int> SaveChangesAsync()
        {
            return await context.SaveChangesAsync();
        }
    }
}
