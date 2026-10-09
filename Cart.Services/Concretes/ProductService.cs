using AutoMapper;
using Cart.Domain.Entities;
using Cart.Persistence.Repositories;
using Cart.Services.Abstracts;
using Cart.Services.DTOs.Product;

namespace Cart.Services.Concretes
{
    public class ProductService(IRepository<Product, Guid> repository, IMapper mapper, IServiceProvider serviceProvider) : CrudService<Product, ProductDto, ProductCreateDto, ProductUpdateDto, Guid>(repository, mapper, serviceProvider), IProductService
    {
    }
}
