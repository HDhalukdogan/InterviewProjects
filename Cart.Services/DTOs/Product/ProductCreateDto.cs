using Cart.Domain.Entities;
using Cart.Services.DTOs.CartItem;

namespace Cart.Services.DTOs.Product
{
    public class ProductCreateDto
    {
        public required string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public Category Category { get; set; } = Category.Electronics;
    }
}


