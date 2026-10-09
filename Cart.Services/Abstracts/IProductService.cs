using Cart.Domain.Entities;
using Cart.Services.DTOs.Product;

namespace Cart.Services.Abstracts
{
    public interface IProductService : ICrudService<Product, ProductDto, ProductCreateDto, ProductUpdateDto, Guid>
    {
    }
}
