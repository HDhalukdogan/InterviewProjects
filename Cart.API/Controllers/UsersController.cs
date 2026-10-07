using Cart.Domain.Entities;
using Cart.Services.Abstracts;
using Microsoft.AspNetCore.Mvc;

namespace Cart.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IUserService service) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get() 
        { 
            var response = await service.GetAllAsync();
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Post(User input)
        {
            await service.CreateAsync(input);
            return Ok();
        }
    }
}
