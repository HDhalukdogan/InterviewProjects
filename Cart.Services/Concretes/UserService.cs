using Cart.Domain.Entities;
using Cart.Persistence.Repositories;
using Cart.Services.Abstracts;

namespace Cart.Services.Concretes
{
    public class UserService(IRepository<User> repository): CrudService<User>(repository), IUserService
    {

    }
}
