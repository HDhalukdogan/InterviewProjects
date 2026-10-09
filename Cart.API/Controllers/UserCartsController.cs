using Cart.Domain.Entities;
using Cart.Services.Abstracts;
using Cart.Services.DTOs.CartItem;
using Cart.Services.DTOs.UserCart;
using Microsoft.AspNetCore.Mvc;

namespace Cart.API.Controllers
{
    public class UserCartsController(IUserCartService userCartService)
        : BaseCrudController<UserCart, UserCartDto, UserCartCreateDto, UserCartUpdateDto, Guid>(userCartService)
    {
        [HttpPut("addItemToUserCart")]
        public async Task<IActionResult> AddItemToUserCart(Guid userId, CartItemCreateDto dto)
        {
            var response = await userCartService.AddCartItemToUserCartAsync(userId, dto);

            return Ok(response);
        }
    }
}
