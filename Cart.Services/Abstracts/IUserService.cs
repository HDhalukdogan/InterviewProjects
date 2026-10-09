using Cart.Domain.Entities;
using Cart.Services.DTOs.User;

namespace Cart.Services.Abstracts
{
    public interface IUserService : ICrudService<User, UserDto, UserCreateDto, UserUpdateDto, Guid>
    {
    }
}
