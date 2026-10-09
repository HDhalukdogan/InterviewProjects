using AutoMapper;
using Cart.Domain.Entities;
using Cart.Persistence.Repositories;
using Cart.Services.Abstracts;
using Cart.Services.DTOs.User;

namespace Cart.Services.Concretes
{
    public class UserService(IRepository<User, Guid> repository, IMapper mapper) : CrudService<User, UserDto, UserCreateDto, UserUpdateDto, Guid>(repository, mapper), IUserService
    {
    }
}
