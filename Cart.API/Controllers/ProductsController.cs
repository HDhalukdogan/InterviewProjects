using Cart.Domain.Entities;
using Cart.Services.Abstracts;
using Cart.Services.DTOs.Product;

namespace Cart.API.Controllers
{
    public class ProductsController(IProductService productService)
        : BaseCrudController<Product, ProductDto, ProductCreateDto, ProductUpdateDto, Guid>(productService)
    {
    }
}
