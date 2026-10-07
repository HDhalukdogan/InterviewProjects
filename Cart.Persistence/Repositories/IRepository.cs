using Cart.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cart.Persistence.Repositories
{
    public interface IRepository<T> where T : BaseEntity<Guid>
    {
        public DbSet<T> Table { get;}
        Task<int> SaveChangesAsync();
    }
}
