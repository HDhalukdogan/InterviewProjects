using Cart.Domain.Entities;

namespace Cart.Services.DTOs.Product
{
    public class ProductDto : BaseDto<Guid>
    {
        public required string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public Category Category { get; set; } = Category.Electronics;
    }
}
