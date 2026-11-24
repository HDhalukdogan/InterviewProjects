using AutoMapper;
using Cart.Domain.Entities;
using Cart.Persistence.Repositories;
using Cart.Services.Abstracts;
using Cart.Services.DTOs.User;
using Microsoft.EntityFrameworkCore;

namespace Cart.Services.Concretes
{
    public class UserService(IRepository<User, Guid> repository, IMapper mapper, IServiceProvider serviceProvider)
        : CrudService<User, UserDto, UserCreateDto, UserUpdateDto, Guid>(repository, mapper, serviceProvider), IUserService
    {
        public async Task<UserDto> EnsureUserExistsAsync(Guid userId, string fullName)
        {
            var existingUser = await Repository.FirstOrDefaultAsync(u => u.Id == userId);
            if (existingUser != null)
            {
                return Mapper.Map<UserDto>(existingUser);
            }

            var newUser = new User
            {
                Id = userId,
                FullName = !string.IsNullOrWhiteSpace(fullName) ? fullName : "Identity User"
            };

            await Repository.AddAsync(newUser);
            await CompleteAsync();

            return Mapper.Map<UserDto>(newUser);
        }
    }
}
