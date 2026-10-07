using Cart.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cart.Persistence.Contexts
{
    public class CartContext(DbContextOptions<CartContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<UserCart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
    }
}
