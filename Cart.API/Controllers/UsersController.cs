using Cart.Domain.Entities;
using Cart.Services.Abstracts;
using Cart.Services.DTOs.User;

namespace Cart.API.Controllers
{
    public class UsersController(IUserService userService)
        : BaseCrudController<User, UserDto, UserCreateDto, UserUpdateDto, Guid>(userService)
    {
    }
}
