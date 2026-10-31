using AutoMapper;
using Cart.Domain.Entities;
using Cart.Persistence.Repositories;
using Cart.Services.Abstracts;
using Cart.Services.DTOs.TestEntity;

namespace Cart.Services.Concretes
{
    public class TestEntityService(
        IRepository<TestEntity, Guid> repo, IMapper mapper, IServiceProvider serviceProvider)
        : CrudService<TestEntity, TestEntityDto, TestEntityCreateDto, TestEntityUpdateDto, Guid>(repo, mapper, serviceProvider), 
          ITestEntityService
    {
    }
}