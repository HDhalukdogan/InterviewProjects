using Cart.Domain.Entities;
using Cart.Services.DTOs.TestEntity;

namespace Cart.Services.Abstracts
{
    public interface ITestEntityService 
        : ICrudService<TestEntity, TestEntityDto, TestEntityCreateDto, TestEntityUpdateDto, Guid>
    {
    }
}