using Cart.Domain.Entities;
using Cart.Services.DTOs.CartItem;
using Cart.Services.DTOs.UserCart;

namespace Cart.Services.Abstracts
{
    public interface IUserCartService : ICrudService<UserCart, UserCartDto, UserCartCreateDto, UserCartUpdateDto, Guid>
    {
        Task<UserCartDto?> AddCartItemToUserCartAsync(Guid userId, CartItemCreateDto cartItemCreateDto);
    }
}
