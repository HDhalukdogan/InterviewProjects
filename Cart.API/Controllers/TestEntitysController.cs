using Cart.Domain.Entities;
using Cart.Services.Abstracts;
using Cart.Services.DTOs.TestEntity;
using Microsoft.AspNetCore.Mvc;

namespace Cart.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestEntitysController(ITestEntityService service)
        : BaseCrudController<TestEntity, TestEntityDto, TestEntityCreateDto, TestEntityUpdateDto, Guid>(service)
    {
    }
}