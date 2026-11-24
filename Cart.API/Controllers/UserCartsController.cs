using Cart.Domain.Entities;
using Cart.Services.Abstracts;
using Cart.Services.DTOs.CartItem;
using Cart.Services.DTOs.UserCart;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Cart.API.Controllers
{
    public class UserCartsController(IUserCartService userCartService)
        : BaseCrudController<UserCart, UserCartDto, UserCartCreateDto, UserCartUpdateDto, Guid>(userCartService)
    {
        // JWT çerezinden okunan kimlik bilgisini test ve doğrulama endpoint'i
        [HttpGet("user-info")]
        public IActionResult GetUserInfo()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return Ok(new
                {
                    isAuthenticated = true,
                    userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                    email = User.FindFirst(ClaimTypes.Email)?.Value ?? User.Identity.Name
                });
            }
            return Ok(new { isAuthenticated = false, message = "JWT çerezi veya oturum bulunamadı." });
        }

        [HttpPut("addItemToUserCart")]
        public async Task<IActionResult> AddItemToUserCart(CartItemCreateDto dto, [FromQuery] Guid? userId = null)
        {
            // Kullanıcı JWT ile giriş yapmışsa kimliğini doğrudan JWT claim'inden al
            Guid targetUserId;
            if (userId.HasValue && userId.Value != Guid.Empty)
            {
                targetUserId = userId.Value;
            }
            else
            {
                var claimUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(claimUserId, out targetUserId))
                {
                    // Giriş yapılmamışsa geriye dönük uyumluluk için varsayılan id
                    targetUserId = Guid.Parse("01a116bd-ef79-7551-8a1f-819096434c1e");
                }
            }

            var response = await userCartService.AddCartItemToUserCartAsync(targetUserId, dto);
            return Ok(response);
        }
    }
}
